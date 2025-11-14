using ExportDXF_Kompas.Properties;
using Kompas6API5;
using Kompas6Constants;
using KompasAPI7;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Text.Json;
using System.IO.Ports;
using System.Threading;
using System.Security.Cryptography;

namespace ExportDXF_Kompas
{
    public partial class MainForm: System.Windows.Forms.Form
    {
        private readonly string settingsPath = Path.Combine(
            System.Windows.Forms.Application.StartupPath, "settings.json");
        private readonly KompasService kompas;
        private readonly Settings settings;
        private readonly Inform inform = new Inform();

        public MainForm()
        {
            
            InitializeComponent();
            settings = LoadSettings(); // ← загружаем настройки при запуске
            StartSettingsMonitor();
            setCheckBoxs();
            textNameFile_TextChanged();
            textNameFile.TextChanged += new System.EventHandler(this.textNameFile_TextChanged);
            textSeparator.TabIndexChanged += new System.EventHandler(this.textNameFile_TextChanged);
            textBoxReplaceOut.TabIndexChanged += new System.EventHandler(this.textNameFile_TextChanged);
            textBoxReplaceIn.TabIndexChanged += new System.EventHandler(this.textNameFile_TextChanged);
            kompas = new KompasService(settings, inform);
            bool kompasConnect = kompas.Connect();
            toolStripStatusLabel.Text = kompasConnect ? "✅ Подключено к КОМПАС" : "❌ КОМПАС не запущен";
            toolStripStatusLabel.ForeColor = kompasConnect ? Color.Green : Color.Red;
            butScan.Enabled = kompasConnect;
        }        

        private async void butScan_Click(object sender, EventArgs e)
        {

            inform.Info = "";
            inform.Warning = "";

            toolStripStatusLabel.Text = "🔄 Сканирование...";
            toolStripStatusLabel.ForeColor = Color.Blue;
            butScan.Enabled = false;
            treeParts.Nodes.Clear();

            // включаем анимацию
            toolStripProgressBar.Style = ProgressBarStyle.Marquee;
            toolStripProgressBar.MarqueeAnimationSpeed = 30;

            // теперь можно обновлять UI
            var nodes = await Task.Run(() => kompas.Scan());
            treeParts.Nodes.AddRange(nodes);

            // выключаем анимацию
            toolStripProgressBar.Style = ProgressBarStyle.Blocks;
            toolStripProgressBar.MarqueeAnimationSpeed = 0;

            toolStripStatusLabel.Text = "✅ Сканирование завершено";
            toolStripStatusLabel.ForeColor = Color.Green;
            butScan.Enabled = true;
            if (nodes.Count() == 0)
            {
                toolStripStatusLabel.Text = "❌ Сканирование не удачно";
                toolStripStatusLabel.ForeColor = Color.Red;
                return;
            }            
            checkedExport();
            textNameFile_TextChanged();

        }

        private void treeParts_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var part = e.Node.Tag as PartInfo;
            var node = e.Node;
            if (part.Assembly) return;
            part.Selected = !part.Selected;
            checkedExport(part, node);
        }

        private void treeParts_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var part = e.Node.Tag as PartInfo;
            var node = e.Node;

            label_Info.Visible = false;
            flagExportDXF.Visible = !part.Assembly; flagExportDXF.Tag = node; flagExportDXF.Checked = part.Selected;
            label_type.Visible = true; text_type.Visible = true; text_type.Text = part.Type.ToString();//text_type.Text = part.Assembly ? "Сборка" : part.IsSheet ? "Листовая деталь" : "Деталь";
            label_name.Visible = true; text_name.Visible = true; text_name.Text = part.Name;
            label_marking.Visible = true; text_marking.Visible = true; text_marking.Text = part.Marking;
            label_count.Visible = !part.Assembly; text_count.Visible = !part.Assembly; text_count.Text = part.Count.ToString();
            label_thickness.Visible = !part.Assembly && part.IsSheet; text_thickness.Visible = !part.Assembly && part.IsSheet; text_thickness.Text = part.Thickness.ToString("F2");
            label_bendCount.Visible = !part.Assembly && part.IsSheet; text_bendCount.Visible = !part.Assembly && part.IsSheet; text_bendCount.Text = part.BendCount.ToString();
            combo_view.Items.Clear(); combo_view.Items.AddRange(part.Projections.ToArray());
            label_view.Visible = !part.Assembly; combo_view.Visible = !part.Assembly; combo_view.Text = part.View; combo_view.Enabled = combo_view.Items.Count > 0;
            buttonRef_view.Visible = !part.Assembly; buttonRef_view.Tag = part;

            int hwnd = pictureBox.Handle.ToInt32();
            kompas.getApp5().ksDrawKompasDocument(hwnd, part.Part.FileName);

            buttonOpenPart.Visible = true; buttonOpenPart.Tag = part.Part;
            
        }

        private void flagExportDXF_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox flag = sender as CheckBox;
            var node = flag.Tag as TreeNode;
            var part = node.Tag as PartInfo;
            part.Selected = flag.Checked;
            checkedExport(part, node);
        }

        private void checkedExport(PartInfo part = null, TreeNode node = null)
        {
            if ((node != null || part != null) && !part.Assembly)
            {
                // Обновляем текст и чекбокс
                node.Text = (part.Selected ? "✅ " : "❌ ") +
                            $"{part.Marking}-{part.Name}" +
                            (part.Count > 1 ? $" ×{part.Count}" : "");

                flagExportDXF.Checked = part.Selected;
            }
            // 🔍 Проверяем рекурсивно все ноды
            PartInfo _part = HasSelectedNodes(treeParts.Nodes);
            bool anySelected = _part != null;
            if (!anySelected) { button_Export.Enabled = anySelected; return; }
            
            FileInfo file = new FileInfo(_part.Part.FileName);
            listProperties.Tag = _part;
            textBoxPath.Text = file.DirectoryName;
            button_Export.Enabled = anySelected;
            
           

        }

        private PartInfo HasSelectedNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode n in nodes)
            {
                if (n.Tag is PartInfo info && info.Selected)
                    return info;

                // рекурсивный вызов для потомков
                PartInfo part = HasSelectedNodes(n.Nodes);
                if (part != null)
                    return part;
            }
            return null;
        }

        private void buttonPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = textBoxPath.Text;
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBoxPath.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void buttonRef_view_Click(object sender, EventArgs e)
        {
            try
            {
                var btn = sender as Button;
                var info = btn.Tag as PartInfo;
                var part = info.Part;

                var filePath = part.FileName;
                if (System.IO.File.Exists(filePath))
                {
                    IApplication app7 = kompas.getApp();
                    var previousDoc = (IKompasDocument3D)app7.ActiveDocument;                    
                    var partDoc = (IKompasDocument3D)app7.Documents.Open(filePath, false, false);
                    bool _new = previousDoc.PathName == partDoc.PathName;
                    var doc3d1 = partDoc as IKompasDocument3D1;
                    var viewMgr = doc3d1.ViewProjectionManager;

                    info.Projections.Clear();

                    for (int i = 1; i < viewMgr.Count; i++)
                    {
                        var view = viewMgr.ViewProjection[i];
                        var name = view.Name;
                        if (name == "Развертка") name = $"#{name}";
                        info.Projections.Add(name);
                    }
                    combo_view.Items.Clear();
                    combo_view.Items.AddRange(info.Projections.ToArray());
                    combo_view.Text = info.View;
                    combo_view.Enabled = combo_view.Items.Count > 0; 

                    if (!previousDoc.Active) partDoc.Close(DocumentCloseOptions.kdDoNotSaveChanges);
                }
            }
            catch (Exception)
            {
            }
            
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (settings == null) return;

            if (sender is CheckBox check)
            {
                switch (check.Name)
                {
                    case "checkBoxBreakLink":
                        settings.BreakLink = check.Checked;
                        checkBoxRemoveOuterDiameter.Enabled = check.Checked;
                        break;

                    case "checkBoxCenterLinesVisible":
                        settings.CenterLinesVisible = check.Checked;
                        break;

                    case "checkBoxBendsLinesVisible":
                        settings.BendsLinesVisible = check.Checked;
                        break;

                    case "checkBoxBreakLinesVisible":
                        settings.BreakLinesVisible = check.Checked;
                        break;

                    case "checkBoxDisignation":
                        settings.Disignation = check.Checked;
                        break;

                    case "checkBoxCreateViewElements":
                        settings.CreateViewElements = check.Checked;
                        break;

                    case "checkBoxRemoveOuterDiameter":
                        settings.RemoveOuterDiameter = check.Checked;
                        break;
                }
                SaveSettings();
            }
        }

        private void setCheckBoxs()
        {
            checkBoxBreakLink.Checked = settings.BreakLink;
            checkBoxRemoveOuterDiameter.Enabled = settings.BreakLink;
            checkBoxCenterLinesVisible.Checked = settings.CenterLinesVisible;
            checkBoxBendsLinesVisible.Checked = settings.BendsLinesVisible;
            checkBoxBreakLinesVisible.Checked = settings.BreakLinesVisible;
            checkBoxDisignation.Checked = settings.Disignation;
            checkBoxCreateViewElements.Checked = settings.CreateViewElements;
            checkBoxRemoveOuterDiameter.Checked = settings.RemoveOuterDiameter;

        }
        private Settings LoadSettings()
        {
            try
            {
                if (File.Exists(settingsPath))
                {
                    string json = File.ReadAllText(settingsPath);
                    var loaded = JsonSerializer.Deserialize<Settings>(json) ?? new Settings();

                    
                    // === Применяем настройки к UI ===
                    textNameFile.Text = loaded.Sample ?? "";
                    textSeparator.Text = loaded.Separator ?? "_";
                    //loaded.Info = "";
                    //loaded.Warning = "";

                    checkBoxBreakLink.Checked = loaded.BreakLink;
                    checkBoxCenterLinesVisible.Checked = loaded.CenterLinesVisible;
                    checkBoxBendsLinesVisible.Checked = loaded.BendsLinesVisible;
                    checkBoxBreakLinesVisible.Checked = loaded.BreakLinesVisible;
                    checkBoxDisignation.Checked = loaded.Disignation;
                    checkBoxCreateViewElements.Checked = loaded.CreateViewElements;
                    checkBoxRemoveOuterDiameter.Checked = loaded.RemoveOuterDiameter;

                    // === Загружаем шаблоны (если есть) ===
                    listBoxSaveSimple.Items.Clear();
                    if (loaded.Templates != null && loaded.Templates.Count > 0)
                    {
                        foreach (var t in loaded.Templates)
                            listBoxSaveSimple.Items.Add(t.Name);

                        var def = loaded.Templates.FirstOrDefault(t => t.IsDefault);
                        if (def != null)
                            loaded.DefaultTemplateName = def.Name;

                        listBoxSaveSimple.Enabled = true;
                    }
                    else listBoxSaveSimple.Items.Add("пусто");

                    // === Применяем шаблон по умолчанию (если задан) ===
                    if (!string.IsNullOrWhiteSpace(loaded.DefaultTemplateName))
                    {
                        var def = loaded.Templates.FirstOrDefault(t => t.Name == loaded.DefaultTemplateName);
                        if (def != null)
                            LoadTemplate(def);

                        MarkDefaultTemplate(loaded.DefaultTemplateName);
                    }

                    // === Проверяем активность кнопки "Сохранить шаблон" ===
                    buttonSaveSimple.Enabled = !string.IsNullOrWhiteSpace(textBoxNameSimple.Text);

                    // === Подписываемся на изменение имени шаблона (если ещё не подписан) ===
                    textBoxNameSimple.TextChanged -= textBoxNameSimple_TextChanged;
                    textBoxNameSimple.TextChanged += textBoxNameSimple_TextChanged;

                    return loaded;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"⚠️ Ошибка загрузки настроек: {ex.Message}",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // === Если файла нет или ошибка ===
            listBoxSaveSimple.Items.Clear();
            listBoxSaveSimple.Items.Add("Пусто");
            listBoxSaveSimple.Enabled = false;

            buttonSaveSimple.Enabled = !string.IsNullOrWhiteSpace(textBoxNameSimple.Text);

            return new Settings
            {
                BreakLink = true,
                CenterLinesVisible = true,
                BendsLinesVisible = true,
                BreakLinesVisible = false,
                Disignation = false,
                CreateViewElements = false,
                Separator = "_",
                Sample = "{ИмяФайлаОриг}"
            };
        }


        private void SaveSettings()
        {
            try
            {
                // === Обновляем объект settings перед сохранением ===
                settings.Sample = textNameFile.Text;
                settings.Separator = textSeparator.Text;
                //settings.Info = "";
                //settings.Warning = "";

                // Сохраняем поля ReplaceIn/Out если они есть
                // (могут быть не в шаблоне по умолчанию)
                // По желанию можно добавить автосохранение последнего шаблона

                string json = JsonSerializer.Serialize(settings,
                    new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(settingsPath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"⚠️ Ошибка сохранения настроек: {ex.Message}",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void combo_view_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            PartInfo info = buttonRef_view.Tag as PartInfo;
            TreeNode node = flagExportDXF.Tag as TreeNode;
            string selectedItem = combo.SelectedItem.ToString();
            bool newView = info.View != selectedItem;
            info.View = selectedItem;
            
            if (newView)
                node.ForeColor = Color.Chocolate;

        }

        private void buttonOpenPart_Click(object sender, EventArgs e)
        {
            Button but = sender as Button;
            IPart7 part = but.Tag as IPart7;
            var app = kompas.getApp();
            var doc = (IKompasDocument3D)app.Documents.Open(part.FileName, true);
            //var doc = (IKompasDocument3D)app.ActiveDocument;
            doc.Active = true;
            app.Visible = true;

        }

        private void toolStripStatusLabel_Click(object sender, EventArgs e)
        {
            switch (toolStripStatusLabel.Text)
            {
                case "❌ КОМПАС не запущен":
                    bool kompasConnect = kompas.Connect();
                    toolStripStatusLabel.Text = kompasConnect ? "✅ Подключено к КОМПАС" : "❌ КОМПАС не запущен";
                    toolStripStatusLabel.ForeColor = kompasConnect ? Color.Green : Color.Red;
                    butScan.Enabled = kompasConnect;
                    break;
            }
        }

        private void textNameFile_TextChanged(object sender = null, EventArgs e = null)
        {
            try
            {
                var part = listProperties.Tag as PartInfo; 

                if (part == null)
                {
                    part = new PartInfo
                    {
                        Name = "Имя",
                        Marking = "Обозначение",
                        Thickness = 0,
                        HasUnfold = false,
                        EmbodimentIndex = 1,
                        FilePath = "ОригинальныйФайл.m3d",
                        View = "#Вид1"
                    };
                }

                previewPart.Text = $"[папкаСохраненияDXF]{BuildFileName(part)}";
                settings.Sample = textNameFile.Text;
                SaveSettings();
            }
            catch (Exception ex)
            {
                previewPart.Text = $"⚠️ Ошибка: {ex.Message}";
            }
        }



        private void listProperties_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listProperties.SelectedItem != null)
            {
                string selectedTag = listProperties.SelectedItem.ToString();

                // Вставляем в конец текстового поля
                textNameFile.Text += selectedTag;

                // Перемещаем курсор в конец
                textNameFile.SelectionStart = textNameFile.Text.Length;
                textNameFile.SelectionLength = 0;

                // Фокус обратно в текстовое поле
                textNameFile.Focus();
            }
        }

        private void button_Export_Click(object sender, EventArgs e) {

            Export_dxf();

        }

        private async void Export_dxf()
        {
            
            try
            {
                // собираем все узлы с PartInfo рекурсивно
                var allNodes = GetAllNodes(treeParts.Nodes);
                var nodes = allNodes
                    .Where(n => n.Tag is PartInfo info && info.Selected)
                    .ToList();

                if (nodes.Count == 0)
                {
                    MessageBox.Show("Нет выбранных деталей для экспорта.", "Экспорт DXF", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                toolStripStatusLabel.Text = "💾 Экспорт деталей в DXF...";
                toolStripStatusLabel.ForeColor = Color.Blue;
                toolStripProgressBar.Value = 0;
                toolStripProgressBar.Maximum = nodes.Count;
                toolStripProgressBar.Style = ProgressBarStyle.Blocks;

                int count = 0;
                string baseFolder = textBoxPath.Text;

                await Task.Run(() =>
                {
                    foreach (TreeNode n in nodes)
                    {
                        if (n.Tag is PartInfo info)
                        {
                            string fullPath = $"{baseFolder}{BuildFileName(info)}";

                            // создаём каталог, если его нет
                            string dir = Path.GetDirectoryName(fullPath);
                            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                                Directory.CreateDirectory(dir);

                            bool ok = kompas.ExportToDxf(info, info.EmbodimentIndex, fullPath);
                            if (ok)
                                Interlocked.Increment(ref count);

                            Invoke(new Action(() =>
                            {
                                toolStripProgressBar.Value = Math.Min(toolStripProgressBar.Value + 1, toolStripProgressBar.Maximum);
                                toolStripStatusLabel.Text = $"💾 Сохранено {toolStripProgressBar.Value}/{toolStripProgressBar.Maximum}";
                                toolStripStatusLabel.ForeColor = Color.Blue;
                            }));
                        }
                    }
                });

                toolStripProgressBar.Value = toolStripProgressBar.Maximum;
                toolStripStatusLabel.Text = $"✅ Экспорт завершён. Сохранено {count} DXF.";
                toolStripStatusLabel.ForeColor = Color.Green;

                var result = MessageBox.Show(
                    $"Сохранено {count} DXF файлов.\nОткрыть папку экспорта?",
                    "Экспорт завершён",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start("explorer.exe", baseFolder);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Не удалось открыть папку:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"⚠️ Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // === Рекурсивный сбор всех узлов ===
        private List<TreeNode> GetAllNodes(TreeNodeCollection nodes)
        {
            var result = new List<TreeNode>();
            foreach (TreeNode node in nodes)
            {
                result.Add(node);
                if (node.Nodes.Count > 0)
                    result.AddRange(GetAllNodes(node.Nodes));
            }
            return result;
        }


        public string BuildFileName(PartInfo part, int exportIndex = 0)
        {
            try
            {
                bool prevTokenEmpty = false;
                string separator = textSeparator.Text ?? "";
                string pattern = string.IsNullOrWhiteSpace(textNameFile.Text)
                    ? "{ИмяФайлаОриг}.dxf"
                    : textNameFile.Text;

                // Карта значений
                var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "{ИмяФайлаОриг}", Path.GetFileNameWithoutExtension(part?.FilePath ?? "") },
                    { "{Имя}", part?.Name ?? "" },
                    { "{Обазначение}", part?.Marking ?? "" },
                    { "{Толщина металла}", (part?.Thickness ?? 0) > 0 ? part!.Thickness.ToString("0.##") : "" },
                    { "{Развертка}", part?.HasUnfold == true ? "Развертка" : "" },
                    { "{Вид}", string.IsNullOrWhiteSpace(part?.View) ? "" : part!.View },
                    { "{День}", DateTime.Now.Day.ToString("00") },
                    { "{Месяц}", DateTime.Now.Month.ToString("00") },
                    { "{Год}", DateTime.Now.Year.ToString() },
                    { "{Номер}", (exportIndex + 1).ToString("000") } // 👈 порядковый номер
                };

                // Добавляем индекс исполнения к {ИмяФайлаОриг}, если > 0
                if ((part?.EmbodimentIndex ?? 0) > 0)
                {
                    var baseName = map["{ИмяФайлаОриг}"];
                    map["{ИмяФайлаОриг}"] = $"{baseName}_{part!.EmbodimentIndex}";
                }

                // Токенизация: находим все {…} и собираем “литералы” между ними
                var rx = new System.Text.RegularExpressions.Regex(@"\{[^}]+\}");
                var m = rx.Matches(pattern);

                var sb = new StringBuilder(pattern.Length + 32);
                int last = 0;
                bool hasContent = false; // было ли что-то “содержательное” до текущей позиции

                // Функция, добавляет текст и помечает, что контент есть (если не только слеши/пробелы/разделители)
                void AppendAndMark(string text)
                {
                    if (string.IsNullOrEmpty(text))
                        return;

                    sb.Append(text);

                    // Контентом считаем любой символ, кроме \ / _ - и пробелов
                    if (text.Any(c => !(c == '\\' || c == '/' || c == '_' || c == '-' || char.IsWhiteSpace(c))))
                        hasContent = true;
                }

                foreach (System.Text.RegularExpressions.Match t in m)
                {
                    // Литерал перед токеном
                    if (t.Index > last)
                    {
                        var literal = pattern.Substring(last, t.Index - last);
                        AppendAndMark(literal);
                    }

                    var token = t.Value;

                    if (token.Equals("{Разделитель}", StringComparison.OrdinalIgnoreCase))
                    {
                        // Разделитель ставим ТОЛЬКО если:
                        // 1) уже есть содержательный контент
                        // 2) предыдущий токен НЕ был пустым
                        if (hasContent && !prevTokenEmpty)
                            sb.Append(separator);

                        // разделитель сам не создаёт контент
                    }
                    else
                    {
                        string val = map.TryGetValue(token, out var v) ? v : "";

                        prevTokenEmpty = string.IsNullOrEmpty(val);

                        AppendAndMark(val);
                    }

                    last = t.Index + t.Length;
                }

                // Хвост после последнего токена — как литерал
                if (last < pattern.Length)
                {
                    var tail = pattern.Substring(last);
                    AppendAndMark(tail);
                }

                // Нормализация слешей: не трогаем \ и / (это каталоги), но всё приводим к '\'
                var result = sb.ToString().Replace("/", "\\");
                result = System.Text.RegularExpressions.Regex.Replace(result, @"\\\\+", "\\");

                // Убираем недопустимые для имени файла символы, кроме каталожных разделителей
                char[] invalid = Path.GetInvalidFileNameChars();
                foreach (char c in invalid)
                {
                    if (c != '\\' && c != '/')
                        result = result.Replace(c.ToString(), "_");
                }

                // Если в начале нет "\", добавим (пусть путь будет относительным к базе экспорта)
                if (!result.StartsWith("\\"))
                    result = "\\" + result;                

                // === ✂️ Пользовательские замены (по позициям в двух списках) ===
                if (!string.IsNullOrWhiteSpace(textBoxReplaceIn?.Text) && !string.IsNullOrWhiteSpace(textBoxReplaceOut?.Text))
                {
                    var listIn = textBoxReplaceIn.Text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                                                      .Select(s => s.Trim())
                                                      .ToList();
                    var listOut = textBoxReplaceOut.Text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                                                       .Select(s => s.Trim())
                                                       .ToList();

                    int count = Math.Min(listIn.Count, listOut.Count);

                    for (int i = 0; i < count; i++)
                    {
                        string from = listIn[i];
                        string to = listOut[i];

                        if (!string.IsNullOrEmpty(from))
                            result = result.Replace(from, to);
                    }
                }

                // Добавляем .dxf при отсутствии
                if (!result.EndsWith(".dxf", StringComparison.OrdinalIgnoreCase))
                    result += ".dxf";

                return result;
            }
            catch (Exception ex)
            {
                return $"⚠️ Ошибка формирования имени: {ex.Message}";
            }
        }

        // === Загрузить шаблон в поля ===
        private void LoadTemplate(FileNameTemplate t)
        {
            textNameFile.Text = t.Pattern;
            textSeparator.Text = t.Separator;
            textBoxReplaceIn.Text = t.ReplaceIn;
            textBoxReplaceOut.Text = t.ReplaceOut;
        }

        // === Собрать шаблон из текущих полей ===
        private FileNameTemplate CollectTemplate()
        {
            return new FileNameTemplate
            {
                Name = textBoxNameSimple.Text.Trim(),
                Pattern = textNameFile.Text,
                Separator = textSeparator.Text,
                ReplaceIn = textBoxReplaceIn.Text,
                ReplaceOut = textBoxReplaceOut.Text
            };
        }

        // === Отметить шаблон по умолчанию жирным ===
        private void MarkDefaultTemplate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;

            for (int i = 0; i < listBoxSaveSimple.Items.Count; i++)
            {
                if (listBoxSaveSimple.Items[i].ToString().Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    listBoxSaveSimple.SelectedIndex = i;
                    break;
                }
            }

            listBoxSaveSimple.Invalidate(); // перерисовать, чтобы применился жирный шрифт
        }

        private void buttonSaveSimple_Click(object sender, EventArgs e)
        {
            string name = textBoxNameSimple.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите имя шаблона перед сохранением.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newTemplate = CollectTemplate();

            // если уже есть шаблон с таким именем — заменяем
            var existing = settings.Templates.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
                settings.Templates.Remove(existing);

            settings.Templates.Add(newTemplate);

            // если стоит галочка “по умолчанию”
            if (checkBoxSimple.Checked)
                settings.DefaultTemplateName = name;

            // обновляем список
            listBoxSaveSimple.Items.Clear();
            foreach (var t in settings.Templates)
                listBoxSaveSimple.Items.Add(t.Name);

            MarkDefaultTemplate(settings.DefaultTemplateName);

            SaveSettings();

            MessageBox.Show("✅ Шаблон сохранён!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void listBoxSaveSimple_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBoxSaveSimple.SelectedItem == null) return;
            string name = listBoxSaveSimple.SelectedItem.ToString();

            if (name == "Пусто") return;

            var t = settings.Templates.FirstOrDefault(x => x.Name == name);
            if (t != null)
                LoadTemplate(t);
        }

        private void textBoxNameSimple_TextChanged(object sender, EventArgs e)
        {
            buttonSaveSimple.Enabled = !string.IsNullOrWhiteSpace(textBoxNameSimple.Text);
        }

        private void menuSetDefault_Click(object sender, EventArgs e)
        {
            if (listBoxSaveSimple.SelectedItem == null) return;
            string name = listBoxSaveSimple.SelectedItem.ToString();
            if (name == "Пусто") return;

            // снимаем флаг "по умолчанию" у всех
            foreach (var t in settings.Templates)
                t.IsDefault = false;

            // отмечаем выбранный
            var selected = settings.Templates.FirstOrDefault(t => t.Name == name);
            if (selected != null)
                selected.IsDefault = true;

            settings.DefaultTemplateName = name;

            SaveSettings();
            MarkDefaultTemplate(name);

            MessageBox.Show($"✅ Шаблон «{name}» установлен по умолчанию.",
                            "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuDeleteTemplate_Click(object sender, EventArgs e)
        {
            if (listBoxSaveSimple.SelectedItem == null) return;
            string name = listBoxSaveSimple.SelectedItem.ToString();
            if (name == "пусто") return;

            var confirm = MessageBox.Show($"Удалить шаблон «{name}»?",
                                          "Подтверждение удаления",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            var existing = settings.Templates.FirstOrDefault(t => t.Name == name);
            if (existing != null)
                settings.Templates.Remove(existing);

            if (settings.DefaultTemplateName == name)
                settings.DefaultTemplateName = "";

            listBoxSaveSimple.Items.Remove(name);

            if (listBoxSaveSimple.Items.Count == 0)
            {
                listBoxSaveSimple.Items.Add("пусто");
                listBoxSaveSimple.Enabled = false;
            }

            SaveSettings();
        }

        private void listBoxSaveSimple_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = listBoxSaveSimple.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    listBoxSaveSimple.SelectedIndex = index;
                }
            }
        }

        private void listBoxSaveSimple_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            string itemName = listBoxSaveSimple.Items[e.Index].ToString();
            bool isDefault = settings.DefaultTemplateName == itemName;

            Font font = isDefault
                ? new Font(e.Font, FontStyle.Bold)
                : e.Font;

            e.DrawBackground();

            using (var brush = new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(itemName, font, brush, e.Bounds);
            }

            e.DrawFocusRectangle();
        }

        private void ВыводDXFПоОригинальномуИмени_Click(object sender, EventArgs e)
        {
            ExportByTemplate(new FileNameTemplate
            {
                Name = "Оригинальное имя",
                Pattern = "{ИмяФайлаОриг}",
                Separator = "_",
                ReplaceIn = "",
                ReplaceOut = ""
            });
        }

        private void ВыводToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            // очищаем старые элементы
            выводToolStripMenuItem.DropDownItems.Clear();

            if (settings.Templates == null || settings.Templates.Count == 0)
            {
                var empty = new ToolStripMenuItem("пусто") { Enabled = false };
                выводToolStripMenuItem.DropDownItems.Add(empty);
                return;
            }

            foreach (var t in settings.Templates)
            {
                var item = new ToolStripMenuItem(t.Name);
                item.Click += (s, _) => ExportByTemplate(t);


                // помечаем шаблон по умолчанию символом ⭐
                if (t.IsDefault)
                    item.Font = new Font(item.Font, FontStyle.Bold);

                выводToolStripMenuItem.DropDownItems.Add(item);
            }
        }

        private void ExportByTemplate(FileNameTemplate template)
        {

            // Подготовка шаблона
            textNameFile.Text = template.Pattern;
            textSeparator.Text = template.Separator;
            textBoxReplaceIn.Text = template.ReplaceIn;
            textBoxReplaceOut.Text = template.ReplaceOut;

            Export_dxf();

        }

        private void contextMenuExport_Opening(object sender, CancelEventArgs e)
        {
            if (contextMenuExport.SourceControl == treeParts)
            {
                // если вызвали на дереве — работаем с выбранным узлом
                var node = treeParts.GetNodeAt(treeParts.PointToClient(Cursor.Position));
                if (node != null)
                    treeParts.SelectedNode = node;
            }
        }

        private void treeParts_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                bool hasSupported =
                    files.Any(f =>
                    {
                        string ext = Path.GetExtension(f);
                        return ext.Equals(".m3d", StringComparison.OrdinalIgnoreCase)
                            || ext.Equals(".a3d", StringComparison.OrdinalIgnoreCase);
                    });

                e.Effect = hasSupported ? DragDropEffects.Copy : DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private async void treeParts_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var files = ((string[])e.Data.GetData(DataFormats.FileDrop))
                    .Where(f =>
                           f.EndsWith(".m3d", StringComparison.OrdinalIgnoreCase) ||
                           f.EndsWith(".a3d", StringComparison.OrdinalIgnoreCase))
                    .Distinct()
                    .ToList();

                if (files.Count == 0)
                    return;

                // Проверка КОМПАС
                if (kompas == null || !kompas.Connect())
                {
                    MessageBox.Show("КОМПАС не запущен.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                toolStripStatusLabel.Text = "🔎 Загрузка файлов…";
                toolStripProgressBar.Style = ProgressBarStyle.Marquee;
                toolStripProgressBar.MarqueeAnimationSpeed = 30;

                treeParts.Nodes.Clear();

                await Task.Run(() =>
                {
                    foreach (var file in files)
                    {
                        var ext = Path.GetExtension(file).ToLowerInvariant();

                        try
                        {
                            TreeNode[] nodes;

                            nodes = kompas.Scan(file);

                            Invoke(new Action(() =>
                            {
                                treeParts.Nodes.AddRange(nodes);
                            }));
                        }
                        catch (Exception exf)
                        {
                            Invoke(new Action(() =>
                                MessageBox.Show($"Ошибка файла {Path.GetFileName(file)}:\n{exf.Message}")));
                        }
                    }
                });

                treeParts.ExpandAll();

                toolStripStatusLabel.Text = $"Готово ({treeParts.Nodes.Count} корневых узлов).";
            }
            finally
            {
                toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                toolStripProgressBar.MarqueeAnimationSpeed = 0;
            }
        }

        private void StartSettingsMonitor()
        {
            Task.Run(() =>
            {
                string lastInfo = "";
                string lastWarn = "";

                while (true)
                {
                    try
                    {
                        // сохраняем текущие значения
                        string warn = inform.Warning?.Trim() ?? "";
                        string info = inform.Info?.Trim() ?? "";

                        // если есть предупреждение — показываем красным
                        if (warn != lastWarn)
                        {
                            lastWarn = warn;
                            Invoke(new Action(() =>
                            {
                                if (!string.IsNullOrEmpty(warn))
                                {
                                    toolStripInfoLabel.Text = "⚠ " + warn;
                                    toolStripInfoLabel.ForeColor = Color.Red;
                                }
                            }));
                        }
                        // если предупреждения нет — показываем Info, если оно есть
                        else if (string.IsNullOrEmpty(warn) && info != lastInfo)
                        {
                            lastInfo = info;
                            Invoke(new Action(() =>
                            {
                                if (!string.IsNullOrEmpty(info))
                                {
                                    toolStripInfoLabel.Text = "ℹ " + info;
                                    toolStripInfoLabel.ForeColor = Color.Orange; // жёлтый
                                }
                                else
                                {
                                    toolStripInfoLabel.Text = "";
                                }
                            }));
                        }
                    }
                    catch
                    {
                        // игнорируем ошибки, чтобы поток не падал
                    }

                    System.Threading.Thread.Sleep(200); // частота обновления 5 раз в секунду
                }
            });
        }

    }
}
