using ExportDXF_Kompas.Properties;
using Kompas6API5;
using Kompas6Constants;
using Kompas6Constants3D;
using KompasAPI7;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ExportDXF_Kompas
{
    public class KompasService
    {
        
        private IApplication app7;
        private KompasObject app5;
        private readonly Settings settings;
        private readonly Inform inform;
        public bool startKompas = false;

        public IApplication getApp() { return app7; }
        public KompasObject getApp5() { return app5; }

        public KompasService(Settings _settings, Inform _inform) { settings = _settings; inform = _inform; }

        public bool StartApp() {
            try
            {
                Type t = Type.GetTypeFromProgID("Kompas.Application.7");
                Activator.CreateInstance(t);                
                return startKompas = Connect();
            }
            catch { return false; }
            
        }

        public bool Connect()
        {
            try
            {
                app7 = (IApplication)Marshal.GetActiveObject("Kompas.Application.7");
                app5 = (KompasObject)Marshal.GetActiveObject("Kompas.Application.5");
                app7.HideMessage = ksHideMessageEnum.ksHideMessageYes;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public TreeNode[] Scan(string filePath = null)
        {
            try
            {
                var doc3d = filePath == null
                    ? (IKompasDocument3D)app7.ActiveDocument
                    : (IKompasDocument3D)app7.Documents.Open(filePath, false, true);

                if (doc3d == null)
                    return Array.Empty<TreeNode>();

                var topPart = doc3d.TopPart;
                if (topPart == null)
                    return Array.Empty<TreeNode>();

                var documentTypeEnum = doc3d.DocumentType;

                // === Если это сборка ===
                if (documentTypeEnum == DocumentTypeEnum.ksDocumentAssembly)
                {
                    var rootInfo = GetPart(topPart);
                    var rootNode = new TreeNode($"{rootInfo.Marking}-{rootInfo.Name}") { Tag = rootInfo };
                    rootNode.BackColor = setColor(rootInfo);
                    rootNode.ImageKey = rootNode.SelectedImageKey = "assembly";

                    BuildTree(topPart, rootNode);
                    return new[] { rootNode };
                }

                // =====================================================================
                // === Если это ОДИНОЧНАЯ ДЕТАЛЬ, но есть несколько исполнений ==========
                // =====================================================================
                var embMgr = topPart as IEmbodimentsManager;
                int embCount = embMgr?.EmbodimentCount ?? 0;

                if (embCount > 1)
                {
                    // Получаем базовое исполнение и формируем root как "сборку"
                    var baseInfo = GetPart(embMgr.Embodiment[0].Part, 0);                    

                    var rootNode = new TreeNode($"{baseInfo.Marking}-{baseInfo.Name}")
                    {
                        Tag = baseInfo
                    };

                    // === РУТ ВЕДЁТ СЕБЯ КАК СБОРКА ===                    
                    rootNode.ImageKey = rootNode.SelectedImageKey = icons(baseInfo);
                    baseInfo.Assembly = true;
                    baseInfo.Selected = false;
                    rootNode.BackColor = setColor(baseInfo);

                    // Добавляем исполнения
                    for (int i = 0; i < embCount; i++)
                    {
                        var info = GetPart(embMgr.Embodiment[i].Part, i);
                        if (info == null) continue;

                        var node = new TreeNode(checkBoxStr(info) + $"{info.Marking}-{info.Name}")
                        {
                            Tag = info
                        };
                        node.BackColor = setColor(info);
                        node.ImageKey = node.SelectedImageKey = icons(info);

                        rootNode.Nodes.Add(node);
                    }

                    return new[] { rootNode };
                }

                // === Если только 1 исполнение (обычная деталь) ===
                var singleInfo = GetPart(topPart, 0);
                if (singleInfo != null)
                {
                    var n = new TreeNode(checkBoxStr(singleInfo) + $"{singleInfo.Marking}-{singleInfo.Name}") { Tag = singleInfo };
                    n.BackColor = setColor(singleInfo);
                    n.ImageKey = n.SelectedImageKey = icons(singleInfo);
                    return new[] { n };
                }
            }
            catch
            {
                MessageBox.Show("⚠️ Ошибка сканирования:\nДокумент не является корректной деталью или сборкой.",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return Array.Empty<TreeNode>();
        }

        private PartInfo GetPart(IPart7 part, int embodimentIndex = 0) {
            try
            {
                double th = 0; int bc = 0;
                string name = part.Name;
                string marking = part.Marking;
                bool isSheet = part.Detail && IsSheetMetal(part, out th, out bc);
                bool hasUnfold = isSheet && HasUnfoldByParameters(part);
                IFeature7 feature7 = (IFeature7)part;

                if (part.FileName == null || part.FileName == "")
                {
                    MessageBox.Show($"⚠️ Похоже деталь не сохранена", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                } 

                PartInfo info = new PartInfo
                {
                    Assembly = !part.Detail,
                    Selected = isSheet,
                    Name = name,
                    Marking = marking,
                    Standart = part.Standard,
                    FilePath = part.FileName,
                    IsSheet = isSheet,
                    HasUnfold = hasUnfold,
                    Thickness = th,
                    BendCount = bc,
                    View = hasUnfold ? "#Развертка" : "#Спереди",
                    EmbodimentIndex = embodimentIndex,
                    Owner = feature7.OwnerFeature?.Label ?? string.Empty,
                    Part = part
                };
                if (isSheet && !hasUnfold && !part.Standard) inform.Info = "Имеются листовые детали без развертки!";

                info.Type = getType(info);

                return info;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"⚠️ Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private bool IsSheetMetal(IPart7 part, out double thickness, out int bendCount)
        {
            thickness = 0.0;
            bendCount = 0;
            bool _isSheetMetal = false;

            try
            {
                var sheet = part as ISheetMetalContainer;
                if (sheet == null) return false;

                var bodies = sheet.SheetMetalBodies;
                if (!(bodies.Count > 0)) return false;

                thickness = bodies.SheetMetalBody[0].Thickness;
                if (thickness > 0) _isSheetMetal = true;
                IFeature7 iFeat = part.Owner;

                //перебираем дерево построения
                foreach (var cFeat in iFeat.SubFeatures[0, true, true])
                {
                    //раскрываем объект дерева (если это возможно)
                    if (cFeat.SubFeatures[0, true, true] != null)
                    {
                        //перебираем состав объекта дерева
                        foreach (var sFeat in cFeat.SubFeatures[0, true, true])
                        {
                            //проверяем тип компонента объекта
                            // 11001 - соответствует (?) сгибу
                            if (sFeat.type == 11001) { _isSheetMetal = true;  bendCount++; }
                        }
                    }
                }
                return _isSheetMetal;
            }

            catch
            {
                return false;
            }
        }

        private bool HasUnfoldByParameters(IPart7 part)
        {
            try
            {
                if (part == null)
                    return false;

                var sheet = part as ISheetMetalContainer;
                if (sheet == null)
                    return false;

                var unfoldParams = sheet.SheetMetalBendUnfoldParameters;
                if (unfoldParams == null)
                    return false;

                bool unfold = unfoldParams.IsCreated;
                return unfold;
            }
            catch
            {
                return false;
            }
        }

        private void BuildTree(IPart7 parentPart, TreeNode parentNode)
        {
            try
            {
                foreach (IPart7 childPart in parentPart.Parts)
                {
                    var info = GetPart(childPart);
                    if (info == null)
                        continue;

                    // === Проверяем, есть ли уже такая деталь (по имени или маркировке)
                    TreeNode existingNode = parentNode.Nodes
                        .Cast<TreeNode>()
                        .FirstOrDefault(n =>
                        {
                            var existingInfo = n.Tag as PartInfo;
                            return existingInfo != null &&
                                   existingInfo.Marking == info.Marking &&
                                   existingInfo.Name == info.Name;                                   
                        });

                    if (existingNode != null)
                    {
                        // уже есть такая — увеличиваем счётчик
                        var existingInfo = existingNode.Tag as PartInfo;
                        existingInfo.Count++;
                        existingNode.Text = (checkBoxStr(existingInfo)) + $"{existingInfo.Marking}-{existingInfo.Name} ×{existingInfo.Count}";
                        existingNode.BackColor = setColor(existingInfo);
                        existingNode.ImageKey = existingNode.SelectedImageKey = icons(existingInfo);
                    }
                    else
                    {
                        // создаём новый узел
                        var node = new TreeNode(checkBoxStr(info) + $"{info.Marking}-{info.Name}") { Tag = info };
                        node.BackColor = setColor(info);
                        node.ImageKey = node.SelectedImageKey = icons(info);
                        parentNode.Nodes.Add(node);

                        // если это подсборка — углубляемся
                        if (!childPart.Detail)
                        {
                            info.Assembly = true;
                            BuildTree(childPart, node);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("⚠️ Ошибка рекурсии: " + ex.Message);
            }
        }

        private string checkBoxStr(PartInfo part) { return !part.Assembly ? (part.Selected ? "✅ " : "❌ ") : ""; }
        private string icons(PartInfo info) { return !info.Assembly ? (!info.Standart ? (info.IsSheet ? (info.HasUnfold ? "sheetUnfold" : "sheet") : "part") : "bolt") : "assembly"; }
        private Color setColor(PartInfo info) { return !info.Assembly ? (!info.Standart ? (info.IsSheet ? (info.HasUnfold ? Color.GreenYellow : Color.Thistle) : Color.Coral) : Color.Beige) : Color.Turquoise; }
        private string getType(PartInfo info) { return !info.Assembly ? (!info.Standart ? (info.IsSheet ? (info.HasUnfold ? "Листовая деталь" : "Листовая деталь") : "Деталь") : "Стандартное изделие") : "Сборка"; }

        public bool ExportToDxf(PartInfo part, int embIndex, string outFile)
        {
            ksDocumentParam param = (ksDocumentParam)app5.GetParamStruct((short)StructType2DEnum.ko_DocumentParam);
            param.type = 1;
            param.Init();

            ksDocument2D doc2D = (ksDocument2D)app5.Document2D();
            doc2D.ksCreateDocument(param);

            IKompasDocument2D kompasDoc2D = (IKompasDocument2D)app7.ActiveDocument;

            var srcFile = part.FilePath;

            if (string.IsNullOrEmpty(srcFile) || !File.Exists(srcFile)) return false;

            var mgr = kompasDoc2D.ViewsAndLayersManager;
            var view = mgr.Views.Add(LtViewType.vt_Arbitrary);
            bool ok = false;

            try
            {
                var assoc = (IAssociationView)view;
                assoc.SourceFileName = srcFile;

                var emb = assoc as IEmbodimentsManager;
                if (emb != null && emb.EmbodimentCount > 0)
                {
                    int idx = Math.Max(0, Math.Min(embIndex, emb.EmbodimentCount - 1));
                    emb.SetCurrentEmbodiment(idx);
                }

                bool updated = false;

                string _view = assoc.ProjectionName;
                _view = part.View;
                try
                {
                    try
                    {
                        assoc.Unfold = part.HasUnfold;
                        assoc.ProjectionName = _view;
                        assoc.Name = _view.Replace("#", "");
                    }
                    catch
                    {
                        assoc.Unfold = false;
                        assoc.ProjectionName = "#Спереди";
                        assoc.Name = "Спереди";
                    }

                    assoc.Angle = 0;
                    assoc.X = 0;
                    assoc.Y = 0;
                    assoc.VisibleLinesStyle = (int)ksCurveStyleEnum.ksCSNormal;
                    assoc.Scale = 1;
                    assoc.CenterLinesVisible = settings.CenterLinesVisible;
                    assoc.BendLinesVisible = settings.BendsLinesVisible;
                    assoc.BreakLinesVisible = settings.BreakLinesVisible;


                    IViewDesignation viewDesignation = view as IViewDesignation;
                    bool viewDesignationVisable = settings.Disignation;

                    viewDesignation.ShowUnfold = viewDesignationVisable;
                    viewDesignation.ShowScale = viewDesignationVisable;
                    viewDesignation.ShowPage = viewDesignationVisable;
                    viewDesignation.ShowAngle = viewDesignationVisable;
                    viewDesignation.ShowZone = viewDesignationVisable;
                    viewDesignation.ShowTurn = viewDesignationVisable;
                    viewDesignation.ShowName = viewDesignationVisable;

                    IAssociationViewElements ass = view as IAssociationViewElements;
                    bool CreateViewElements = settings.CreateViewElements;
                    ass.CreateCentresMarkers = settings.CenterLinesVisible;
                    ass.CreateAxis = settings.CenterLinesVisible;
                    ass.CreateLinearCentres = settings.CenterLinesVisible;
                    ass.CreateCircularCentres = settings.CenterLinesVisible;
                    ass.ProjectAllDesignations = CreateViewElements;
                    ass.HiddenObjectsVisible = CreateViewElements;
                    ass.ProjectAxis = CreateViewElements;
                    ass.ProjectBases = CreateViewElements;
                    ass.ProjectBrandLeaders = CreateViewElements;
                    ass.ProjectDesTables = CreateViewElements;
                    ass.ProjectDesTexts = CreateViewElements;
                    ass.ProjectDimensions = CreateViewElements;
                    ass.ProjectHiddenComponents = CreateViewElements;
                    ass.ProjectLayers = CreateViewElements;
                    ass.ProjectMarkLeaders = CreateViewElements;
                    ass.ProjectPoints = CreateViewElements;
                    ass.ProjectPositions = CreateViewElements;
                    ass.ProjectRoughs = CreateViewElements;
                    ass.ProjectSpecRough = CreateViewElements;
                    ass.ProjectStandartElements = CreateViewElements;
                    ass.ProjectThreads = CreateViewElements;
                    ass.ProjectTolerances = CreateViewElements;

                    assoc.Update();
                    view.Update();
                    updated = true;
                }
                catch
                {
                    assoc.Unfold = false;
                    assoc.ProjectionName = "Спереди";
                    try { assoc.Update(); updated = true; } catch { updated = false; }
                }

                if (!updated) { view.Delete(); return false; }

                // === РАЗРЫВАЕМ СВЯЗЬ С 3D-МОДЕЛЬЮ ===
                if (settings.BreakLink)
                {
                    try
                    {
                        int vnum = view.Number;
                        int vref = doc2D.ksGetViewReference(vnum);
                        if (vref != 0) doc2D.ksDestroyObjects(vref); // Разрушить вид
                        doc2D.ksRebuildDocument();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"⚠️ Ошибка разрушения вида: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    // === убераем мелкие отрезки по гибу
                    // Создаём итератор по всем объектам чертежа
                    ksIterator iter = app5.GetIterator();
                    iter.ksCreateIterator(1, 0);
                    int objRef = iter.ksMoveIterator("F");

                    ksLineSegParam lineParam = (ksLineSegParam)app5.GetParamStruct((short)StructType2DEnum.ko_LineSegParam);

                    List<(int id, LineSeg line)> lines = new();

                    // Проходим по всем линиям                
                    while (objRef != 0)
                    {
                        // Читаем параметры линии
                        lineParam.Init();
                        doc2D.ksGetObjParam(objRef, lineParam, (int)StructType2DEnum.ko_LineSegParam);
                        lines.Add((objRef, new LineSeg(lineParam.x1, lineParam.y1, lineParam.x2, lineParam.y2)));
                        objRef = iter.ksMoveIterator("N");
                    }

                    Console.WriteLine($"📏 Найдено линий: {lines.Count}");

                    // === Объединение ===
                    List<LineSeg> merged = MergeLines(lines.Select(l => l.line).ToList());

                    Console.WriteLine($"✅ После объединения: {merged.Count}");
                    foreach (var l in merged)
                        Console.WriteLine($"→ X1={l.X1:F2}, Y1={l.Y1:F2} → X2={l.X2:F2}, Y2={l.Y2:F2}");

                    // === удаляем старые линии ===
                    foreach (var l in lines)
                        doc2D.ksDeleteObj(l.id);

                    // === рисуем новые объединённые ===
                    foreach (var l in merged)
                        doc2D.ksLineSeg(l.X1, l.Y1, l.X2, l.Y2, 1);

                    doc2D.ksRebuildDocument();
                    Console.WriteLine("✨ Готово! Линии объединены и перерисованы.");

                    // убираем внешний диаметр на окружностях с общей точкой если нужно
                    if (settings.RemoveOuterDiameter)
                    {
                        try
                        {
                            ksIterator iter2 = app5.GetIterator();
                            iter2.ksCreateIterator(2, 0); // 2 = все графические объекты
                            int objRef2 = iter2.ksMoveIterator("F");

                            var circles = new List<CircleInfo>();

                            var circleParam = (ksCircleParam)app5.GetParamStruct((short)StructType2DEnum.ko_CircleParam);
                            var arcParam = (ksArcByAngleParam)app5.GetParamStruct((short)StructType2DEnum.ko_ArcByAngleParam);

                            while (objRef2 != 0)
                            {
                                int type = doc2D.ksGetObjParam(objRef2, null, 0);

                                if (type == 2) // окружность
                                {
                                    circleParam.Init();
                                    doc2D.ksGetObjParam(objRef2, circleParam, (int)StructType2DEnum.ko_CircleParam);
                                    circles.Add(new CircleInfo(objRef2, circleParam.xc, circleParam.yc, circleParam.rad));
                                }
                                else if (type == 3) // дуга
                                {
                                    arcParam.Init();
                                    doc2D.ksGetObjParam(objRef2, arcParam, (int)StructType2DEnum.ko_ArcByAngleParam);
                                    circles.Add(new CircleInfo(objRef2, arcParam.xc, arcParam.yc, arcParam.rad));
                                }

                                objRef2 = iter2.ksMoveIterator("N");
                            }

                            if (circles.Count > 0)
                            {
                                double tol = 0.01;

                                var groups = circles
                                    .GroupBy(c => new CenterKey(c.Xc, c.Yc, tol))
                                    .ToList();

                                foreach (var g in groups)
                                {
                                    var list = g.ToList();
                                    if (list.Count <= 1)
                                        continue;

                                    double minR = list.Min(c => c.Radius);

                                    foreach (var c in list)
                                    {
                                        if (c.Radius > minR + tol)
                                        {
                                            doc2D.ksDeleteObj(c.Id);
                                        }
                                    }
                                }

                                doc2D.ksRebuildDocument();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка удаления окружностей:\n" + ex.Message);
                        }
                    }

                }

                // === СОХРАНЯЕМ DXF ===
                try
                {
                    string dir = Path.GetDirectoryName(outFile);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"⚠️ Ошибка создания каталога:\n{ex.Message}",
                                    "Ошибка",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return false; // без каталога сохранять нет смысла
                }
                ok = doc2D.ksSaveToDXF(outFile);

            }
            finally
            {
                try
                {
                    view.Delete();
                    doc2D.ksCloseDocument();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"⚠️ Ошибка закрытия документа: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return ok;

        }

        class CircleInfo
        {
            public int Id;
            public double Xc, Yc, Radius;

            public CircleInfo(int id, double xc, double yc, double r)
            {
                Id = id;
                Xc = xc;
                Yc = yc;
                Radius = r;
            }
        }

        class CenterKey
        {
            public double X, Y;
            public double Tol;

            public CenterKey(double x, double y, double tol)
            {
                X = x;
                Y = y;
                Tol = tol;
            }

            public override bool Equals(object obj)
            {
                if (obj is CenterKey k)
                    return Math.Abs(X - k.X) < Tol && Math.Abs(Y - k.Y) < Tol;
                return false;
            }

            public override int GetHashCode()
            {
                return 0; // чтобы всегда сравнивать Equals() вручную
            }
        }
        static List<LineSeg> MergeLines(List<LineSeg> input, double angleTolDeg = 0.5, double pointTol = 0.02)
        {
            List<LineSeg> lines = new(input);
            bool changed;
            do
            {
                changed = false;
                List<LineSeg> newLines = new();
                var used = new bool[lines.Count];

                for (int i = 0; i < lines.Count; i++)
                {
                    if (used[i]) continue;
                    var a = lines[i];
                    for (int j = i + 1; j < lines.Count; j++)
                    {
                        if (used[j]) continue;
                        var b = lines[j];
                        double da = Math.Abs(NormDeg(Rad2Deg(a.Angle) - Rad2Deg(b.Angle)));
                        if (da > angleTolDeg && Math.Abs(da - 180) > angleTolDeg)
                            continue; // разный угол

                        if (Dist(a.End, b.Start) < pointTol)
                        {
                            a = new LineSeg(a.X1, a.Y1, b.X2, b.Y2);
                            used[j] = true; changed = true;
                        }
                        else if (Dist(a.Start, b.End) < pointTol)
                        {
                            a = new LineSeg(b.X1, b.Y1, a.X2, a.Y2);
                            used[j] = true; changed = true;
                        }
                        else if (Dist(a.Start, b.Start) < pointTol)
                        {
                            a = new LineSeg(b.X2, b.Y2, a.X2, a.Y2);
                            used[j] = true; changed = true;
                        }
                        else if (Dist(a.End, b.End) < pointTol)
                        {
                            a = new LineSeg(a.X1, a.Y1, b.X1, b.Y1);
                            used[j] = true; changed = true;
                        }
                    }
                    newLines.Add(a);
                    used[i] = true;
                }

                lines = newLines;
            } while (changed);
            return lines;
        }

        static double Rad2Deg(double r) => r * 180.0 / Math.PI;
        static double NormDeg(double d)
        {
            while (d < -180) d += 360;
            while (d > 180) d -= 360;
            return d;
        }
        static double Dist((double x, double y) a, (double x, double y) b)
            => Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));

        class LineSeg
        {
            public double X1, Y1, X2, Y2;
            public double Angle => Math.Atan2(Y2 - Y1, X2 - X1);
            public LineSeg(double x1, double y1, double x2, double y2)
            {
                X1 = x1; Y1 = y1; X2 = x2; Y2 = y2;
            }
            public (double x, double y) Start => (X1, Y1);
            public (double x, double y) End => (X2, Y2);
        }

        public TreeNode[] BuildNodesForM3D(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return Array.Empty<TreeNode>();

            IKompasDocument3D doc3d = null;

            try
            {
                // === Открываем документ ===
                doc3d = (IKompasDocument3D)app7.Documents.Open(filePath, false, true);
                if (doc3d == null)
                    return Array.Empty<TreeNode>();

                var topPart = doc3d.TopPart;
                if (topPart == null)
                    return Array.Empty<TreeNode>();

                // === Тип документа (сборки игнорируем, возвращаем пусто) ===
                if (doc3d.DocumentType == DocumentTypeEnum.ksDocumentAssembly)
                {
                    // Нам нужно работать ТОЛЬКО с одиночными деталями
                    return Array.Empty<TreeNode>();
                }

                // === Одиночная деталь с несколькими исполнениями ===
                List<TreeNode> nodes = new List<TreeNode>();
                var embMgr = topPart as IEmbodimentsManager;
                int embCount = embMgr?.EmbodimentCount ?? 0;

                // нет исполнений → одиночная деталь
                if (embCount == 0)
                {
                    var info = GetPart(topPart, 0);
                    if (info != null)
                    {
                        var node = new TreeNode(
                            checkBoxStr(info) + $"{info.Marking}-{info.Name}"
                        )
                        {
                            Tag = info,
                            BackColor = setColor(info),
                            ImageKey = icons(info),
                            SelectedImageKey = icons(info)
                        };

                        nodes.Add(node);
                    }
                }
                else
                {
                    // есть исполнения
                    for (int i = 0; i < embCount; i++)
                    {
                        var info = GetPart(embMgr.Embodiment[i].Part, i);
                        if (info != null)
                        {
                            var node = new TreeNode(
                                checkBoxStr(info) + $"{info.Marking}-{info.Name}"
                            )
                            {
                                Tag = info,
                                BackColor = setColor(info),
                                ImageKey = icons(info),
                                SelectedImageKey = icons(info)
                            };

                            nodes.Add(node);
                        }
                    }
                }

                return nodes.ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"⚠️ Ошибка чтения файла:\n{filePath}\n\n{ex.Message}",
                    "Ошибка M3D", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return Array.Empty<TreeNode>();
            }
            finally
            {
                // Закрываем документ
                //try { doc3d?.Close(DocumentCloseOptions.kdDoNotSaveChanges); } catch { }
            }
        }

    }
}
