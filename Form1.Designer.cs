using System.Drawing;

namespace ExportDXF_Kompas
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.treeParts = new System.Windows.Forms.TreeView();
            this.contextMenuExport = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.выводDXFПоОригинальномуИмениToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выводToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.пустоToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.butScan = new System.Windows.Forms.Button();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripInfoLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.label_type = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.buttonOpenPart = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.buttonRef_view = new System.Windows.Forms.Button();
            this.label_Info = new System.Windows.Forms.Label();
            this.combo_view = new System.Windows.Forms.ComboBox();
            this.flagExportDXF = new System.Windows.Forms.CheckBox();
            this.label_view = new System.Windows.Forms.Label();
            this.text_bendCount = new System.Windows.Forms.Label();
            this.text_thickness = new System.Windows.Forms.Label();
            this.label_bendCount = new System.Windows.Forms.Label();
            this.text_count = new System.Windows.Forms.Label();
            this.label_thickness = new System.Windows.Forms.Label();
            this.text_name = new System.Windows.Forms.Label();
            this.label_count = new System.Windows.Forms.Label();
            this.text_marking = new System.Windows.Forms.Label();
            this.label_name = new System.Windows.Forms.Label();
            this.label_marking = new System.Windows.Forms.Label();
            this.text_type = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.checkBoxRemoveOuterDiameter = new System.Windows.Forms.CheckBox();
            this.checkBoxCreateViewElements = new System.Windows.Forms.CheckBox();
            this.checkBoxDisignation = new System.Windows.Forms.CheckBox();
            this.checkBoxBreakLinesVisible = new System.Windows.Forms.CheckBox();
            this.checkBoxBendsLinesVisible = new System.Windows.Forms.CheckBox();
            this.checkBoxCenterLinesVisible = new System.Windows.Forms.CheckBox();
            this.checkBoxBreakLink = new System.Windows.Forms.CheckBox();
            this.buttonPath = new System.Windows.Forms.Button();
            this.labelPath = new System.Windows.Forms.Label();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.checkBoxSimple = new System.Windows.Forms.CheckBox();
            this.buttonSaveSimple = new System.Windows.Forms.Button();
            this.labelDescription = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelReplace = new System.Windows.Forms.Label();
            this.labelSeparator = new System.Windows.Forms.Label();
            this.labelNameSimple = new System.Windows.Forms.Label();
            this.labelSample = new System.Windows.Forms.Label();
            this.labelSave = new System.Windows.Forms.Label();
            this.labelProperties = new System.Windows.Forms.Label();
            this.listBoxSaveSimple = new System.Windows.Forms.ListBox();
            this.contextMenuTemplates = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuSetDefault = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDeleteTemplate = new System.Windows.Forms.ToolStripMenuItem();
            this.textBoxReplaceOut = new System.Windows.Forms.TextBox();
            this.textBoxReplaceIn = new System.Windows.Forms.TextBox();
            this.listProperties = new System.Windows.Forms.ListBox();
            this.textBoxNameSimple = new System.Windows.Forms.TextBox();
            this.textSeparator = new System.Windows.Forms.TextBox();
            this.textNameFile = new System.Windows.Forms.TextBox();
            this.previewPart = new System.Windows.Forms.Label();
            this.выводDXFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button_Export = new System.Windows.Forms.Button();
            this.contextMenuExport.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.contextMenuTemplates.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeParts
            // 
            this.treeParts.AllowDrop = true;
            this.treeParts.ContextMenuStrip = this.contextMenuExport;
            this.treeParts.ImageIndex = 0;
            this.treeParts.ImageList = this.imageList;
            this.treeParts.Location = new System.Drawing.Point(1, 30);
            this.treeParts.Name = "treeParts";
            this.treeParts.SelectedImageIndex = 0;
            this.treeParts.Size = new System.Drawing.Size(400, 320);
            this.treeParts.TabIndex = 0;
            this.treeParts.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeParts_NodeMouseClick);
            this.treeParts.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeParts_NodeMouseDoubleClick);
            this.treeParts.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeParts_DragDrop);
            this.treeParts.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeParts_DragEnter);
            // 
            // contextMenuExport
            // 
            this.contextMenuExport.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.выводDXFПоОригинальномуИмениToolStripMenuItem,
            this.выводToolStripMenuItem});
            this.contextMenuExport.Name = "contextMenuExport";
            this.contextMenuExport.Size = new System.Drawing.Size(281, 48);
            this.contextMenuExport.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuExport_Opening);
            // 
            // выводDXFПоОригинальномуИмениToolStripMenuItem
            // 
            this.выводDXFПоОригинальномуИмениToolStripMenuItem.Name = "выводDXFПоОригинальномуИмениToolStripMenuItem";
            this.выводDXFПоОригинальномуИмениToolStripMenuItem.Size = new System.Drawing.Size(280, 22);
            this.выводDXFПоОригинальномуИмениToolStripMenuItem.Text = "Вывод DXF по оригинальному имени";
            this.выводDXFПоОригинальномуИмениToolStripMenuItem.Click += new System.EventHandler(this.ВыводDXFПоОригинальномуИмени_Click);
            // 
            // выводToolStripMenuItem
            // 
            this.выводToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.пустоToolStripMenuItem});
            this.выводToolStripMenuItem.Name = "выводToolStripMenuItem";
            this.выводToolStripMenuItem.Size = new System.Drawing.Size(280, 22);
            this.выводToolStripMenuItem.Text = "Вывод по шаблону";
            this.выводToolStripMenuItem.DropDownOpening += new System.EventHandler(this.ВыводToolStripMenuItem_DropDownOpening);
            // 
            // пустоToolStripMenuItem
            // 
            this.пустоToolStripMenuItem.Enabled = false;
            this.пустоToolStripMenuItem.Name = "пустоToolStripMenuItem";
            this.пустоToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.пустоToolStripMenuItem.Text = "пусто";
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "assembly");
            this.imageList.Images.SetKeyName(1, "bolt");
            this.imageList.Images.SetKeyName(2, "part");
            this.imageList.Images.SetKeyName(3, "sheet");
            this.imageList.Images.SetKeyName(4, "sheetUnfold");
            // 
            // butScan
            // 
            this.butScan.Enabled = false;
            this.butScan.Location = new System.Drawing.Point(8, 3);
            this.butScan.Name = "butScan";
            this.butScan.Size = new System.Drawing.Size(86, 23);
            this.butScan.TabIndex = 2;
            this.butScan.Text = "Сканировать";
            this.butScan.UseVisualStyleBackColor = true;
            this.butScan.Click += new System.EventHandler(this.butScan_Click);
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(141, 17);
            this.toolStripStatusLabel.Text = "❌ КОМПАС не запущен";
            this.toolStripStatusLabel.Click += new System.EventHandler(this.toolStripStatusLabel_Click);
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(300, 16);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.toolStripProgressBar,
            this.toolStripInfoLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 428);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(800, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip";
            // 
            // toolStripInfoLabel
            // 
            this.toolStripInfoLabel.Name = "toolStripInfoLabel";
            this.toolStripInfoLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // label_type
            // 
            this.label_type.AutoSize = true;
            this.label_type.Location = new System.Drawing.Point(407, 30);
            this.label_type.Name = "label_type";
            this.label_type.Size = new System.Drawing.Size(29, 13);
            this.label_type.TabIndex = 5;
            this.label_type.Text = "Тип:";
            this.label_type.Visible = false;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Location = new System.Drawing.Point(0, 33);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(800, 376);
            this.tabControl.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.buttonOpenPart);
            this.tabPage1.Controls.Add(this.pictureBox);
            this.tabPage1.Controls.Add(this.buttonRef_view);
            this.tabPage1.Controls.Add(this.label_Info);
            this.tabPage1.Controls.Add(this.combo_view);
            this.tabPage1.Controls.Add(this.flagExportDXF);
            this.tabPage1.Controls.Add(this.label_view);
            this.tabPage1.Controls.Add(this.text_bendCount);
            this.tabPage1.Controls.Add(this.text_thickness);
            this.tabPage1.Controls.Add(this.label_bendCount);
            this.tabPage1.Controls.Add(this.text_count);
            this.tabPage1.Controls.Add(this.label_thickness);
            this.tabPage1.Controls.Add(this.text_name);
            this.tabPage1.Controls.Add(this.label_count);
            this.tabPage1.Controls.Add(this.text_marking);
            this.tabPage1.Controls.Add(this.label_name);
            this.tabPage1.Controls.Add(this.label_marking);
            this.tabPage1.Controls.Add(this.text_type);
            this.tabPage1.Controls.Add(this.treeParts);
            this.tabPage1.Controls.Add(this.label_type);
            this.tabPage1.Controls.Add(this.butScan);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 350);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "📐 Список деталей";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // buttonOpenPart
            // 
            this.buttonOpenPart.Location = new System.Drawing.Point(709, 195);
            this.buttonOpenPart.Name = "buttonOpenPart";
            this.buttonOpenPart.Size = new System.Drawing.Size(75, 23);
            this.buttonOpenPart.TabIndex = 12;
            this.buttonOpenPart.Text = "Открыть";
            this.buttonOpenPart.UseVisualStyleBackColor = true;
            this.buttonOpenPart.Visible = false;
            this.buttonOpenPart.Click += new System.EventHandler(this.buttonOpenPart_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(410, 223);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(374, 137);
            this.pictureBox.TabIndex = 11;
            this.pictureBox.TabStop = false;
            // 
            // buttonRef_view
            // 
            this.buttonRef_view.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonRef_view.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonRef_view.Location = new System.Drawing.Point(666, 195);
            this.buttonRef_view.Margin = new System.Windows.Forms.Padding(0);
            this.buttonRef_view.Name = "buttonRef_view";
            this.buttonRef_view.Size = new System.Drawing.Size(26, 23);
            this.buttonRef_view.TabIndex = 10;
            this.buttonRef_view.Text = "⟳";
            this.buttonRef_view.UseVisualStyleBackColor = true;
            this.buttonRef_view.Visible = false;
            this.buttonRef_view.Click += new System.EventHandler(this.buttonRef_view_Click);
            // 
            // label_Info
            // 
            this.label_Info.AutoSize = true;
            this.label_Info.Location = new System.Drawing.Point(454, 134);
            this.label_Info.Name = "label_Info";
            this.label_Info.Size = new System.Drawing.Size(294, 13);
            this.label_Info.TabIndex = 9;
            this.label_Info.Text = "Нажмите \"Сканировать\" и выберете элемент из списка";
            // 
            // combo_view
            // 
            this.combo_view.Enabled = false;
            this.combo_view.FormattingEnabled = true;
            this.combo_view.Location = new System.Drawing.Point(528, 196);
            this.combo_view.Name = "combo_view";
            this.combo_view.Size = new System.Drawing.Size(135, 21);
            this.combo_view.TabIndex = 8;
            this.combo_view.Visible = false;
            this.combo_view.SelectionChangeCommitted += new System.EventHandler(this.combo_view_SelectionChangeCommitted);
            // 
            // flagExportDXF
            // 
            this.flagExportDXF.AutoSize = true;
            this.flagExportDXF.Location = new System.Drawing.Point(709, 29);
            this.flagExportDXF.Name = "flagExportDXF";
            this.flagExportDXF.Size = new System.Drawing.Size(83, 17);
            this.flagExportDXF.TabIndex = 7;
            this.flagExportDXF.Text = "Вывод DXF";
            this.flagExportDXF.UseVisualStyleBackColor = true;
            this.flagExportDXF.Visible = false;
            this.flagExportDXF.CheckedChanged += new System.EventHandler(this.flagExportDXF_CheckedChanged);
            // 
            // label_view
            // 
            this.label_view.AutoSize = true;
            this.label_view.Location = new System.Drawing.Point(410, 199);
            this.label_view.Name = "label_view";
            this.label_view.Size = new System.Drawing.Size(91, 13);
            this.label_view.TabIndex = 6;
            this.label_view.Text = "Вид для вывода:";
            this.label_view.Visible = false;
            // 
            // text_bendCount
            // 
            this.text_bendCount.AutoSize = true;
            this.text_bendCount.Location = new System.Drawing.Point(525, 173);
            this.text_bendCount.Name = "text_bendCount";
            this.text_bendCount.Size = new System.Drawing.Size(82, 13);
            this.text_bendCount.TabIndex = 6;
            this.text_bendCount.Text = "Кол-во сгибов:";
            this.text_bendCount.Visible = false;
            // 
            // text_thickness
            // 
            this.text_thickness.AutoSize = true;
            this.text_thickness.Location = new System.Drawing.Point(525, 147);
            this.text_thickness.Name = "text_thickness";
            this.text_thickness.Size = new System.Drawing.Size(102, 13);
            this.text_thickness.TabIndex = 6;
            this.text_thickness.Text = "Толщина металла:";
            this.text_thickness.Visible = false;
            // 
            // label_bendCount
            // 
            this.label_bendCount.AutoSize = true;
            this.label_bendCount.Location = new System.Drawing.Point(410, 173);
            this.label_bendCount.Name = "label_bendCount";
            this.label_bendCount.Size = new System.Drawing.Size(82, 13);
            this.label_bendCount.TabIndex = 6;
            this.label_bendCount.Text = "Кол-во сгибов:";
            this.label_bendCount.Visible = false;
            // 
            // text_count
            // 
            this.text_count.AutoSize = true;
            this.text_count.Location = new System.Drawing.Point(525, 108);
            this.text_count.Name = "text_count";
            this.text_count.Size = new System.Drawing.Size(92, 13);
            this.text_count.TabIndex = 6;
            this.text_count.Text = "Кол-во в сборке:";
            this.text_count.Visible = false;
            // 
            // label_thickness
            // 
            this.label_thickness.AutoSize = true;
            this.label_thickness.Location = new System.Drawing.Point(410, 147);
            this.label_thickness.Name = "label_thickness";
            this.label_thickness.Size = new System.Drawing.Size(102, 13);
            this.label_thickness.TabIndex = 6;
            this.label_thickness.Text = "Толщина металла:";
            this.label_thickness.Visible = false;
            // 
            // text_name
            // 
            this.text_name.AutoSize = true;
            this.text_name.Location = new System.Drawing.Point(525, 56);
            this.text_name.Name = "text_name";
            this.text_name.Size = new System.Drawing.Size(32, 13);
            this.text_name.TabIndex = 6;
            this.text_name.Text = "Имя:";
            this.text_name.Visible = false;
            // 
            // label_count
            // 
            this.label_count.AutoSize = true;
            this.label_count.Location = new System.Drawing.Point(408, 108);
            this.label_count.Name = "label_count";
            this.label_count.Size = new System.Drawing.Size(92, 13);
            this.label_count.TabIndex = 6;
            this.label_count.Text = "Кол-во в сборке:";
            this.label_count.Visible = false;
            // 
            // text_marking
            // 
            this.text_marking.AutoSize = true;
            this.text_marking.Location = new System.Drawing.Point(525, 82);
            this.text_marking.Name = "text_marking";
            this.text_marking.Size = new System.Drawing.Size(77, 13);
            this.text_marking.TabIndex = 6;
            this.text_marking.Text = "Обазначение:";
            this.text_marking.Visible = false;
            // 
            // label_name
            // 
            this.label_name.AutoSize = true;
            this.label_name.Location = new System.Drawing.Point(410, 56);
            this.label_name.Name = "label_name";
            this.label_name.Size = new System.Drawing.Size(32, 13);
            this.label_name.TabIndex = 6;
            this.label_name.Text = "Имя:";
            this.label_name.Visible = false;
            // 
            // label_marking
            // 
            this.label_marking.AutoSize = true;
            this.label_marking.Location = new System.Drawing.Point(410, 82);
            this.label_marking.Name = "label_marking";
            this.label_marking.Size = new System.Drawing.Size(77, 13);
            this.label_marking.TabIndex = 6;
            this.label_marking.Text = "Обазначение:";
            this.label_marking.Visible = false;
            // 
            // text_type
            // 
            this.text_type.AutoSize = true;
            this.text_type.Location = new System.Drawing.Point(525, 30);
            this.text_type.Name = "text_type";
            this.text_type.Size = new System.Drawing.Size(29, 13);
            this.text_type.TabIndex = 5;
            this.text_type.Text = "Тип:";
            this.text_type.Visible = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.checkBoxRemoveOuterDiameter);
            this.tabPage2.Controls.Add(this.checkBoxCreateViewElements);
            this.tabPage2.Controls.Add(this.checkBoxDisignation);
            this.tabPage2.Controls.Add(this.checkBoxBreakLinesVisible);
            this.tabPage2.Controls.Add(this.checkBoxBendsLinesVisible);
            this.tabPage2.Controls.Add(this.checkBoxCenterLinesVisible);
            this.tabPage2.Controls.Add(this.checkBoxBreakLink);
            this.tabPage2.Controls.Add(this.buttonPath);
            this.tabPage2.Controls.Add(this.labelPath);
            this.tabPage2.Controls.Add(this.textBoxPath);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(792, 350);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "⚙️ DXF параметры";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // checkBoxRemoveOuterDiameter
            // 
            this.checkBoxRemoveOuterDiameter.AutoSize = true;
            this.checkBoxRemoveOuterDiameter.Enabled = false;
            this.checkBoxRemoveOuterDiameter.Location = new System.Drawing.Point(142, 248);
            this.checkBoxRemoveOuterDiameter.Name = "checkBoxRemoveOuterDiameter";
            this.checkBoxRemoveOuterDiameter.Size = new System.Drawing.Size(215, 17);
            this.checkBoxRemoveOuterDiameter.TabIndex = 3;
            this.checkBoxRemoveOuterDiameter.Text = "Убрать внешний диаметр у зенковок";
            this.checkBoxRemoveOuterDiameter.UseVisualStyleBackColor = true;
            this.checkBoxRemoveOuterDiameter.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxCreateViewElements
            // 
            this.checkBoxCreateViewElements.AutoSize = true;
            this.checkBoxCreateViewElements.Location = new System.Drawing.Point(142, 225);
            this.checkBoxCreateViewElements.Name = "checkBoxCreateViewElements";
            this.checkBoxCreateViewElements.Size = new System.Drawing.Size(325, 17);
            this.checkBoxCreateViewElements.TabIndex = 3;
            this.checkBoxCreateViewElements.Text = "Передовать в вид элементы (ТТ, позиций, линий-выносок)";
            this.checkBoxCreateViewElements.UseVisualStyleBackColor = true;
            this.checkBoxCreateViewElements.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxDisignation
            // 
            this.checkBoxDisignation.AutoSize = true;
            this.checkBoxDisignation.Location = new System.Drawing.Point(142, 202);
            this.checkBoxDisignation.Name = "checkBoxDisignation";
            this.checkBoxDisignation.Size = new System.Drawing.Size(217, 17);
            this.checkBoxDisignation.TabIndex = 3;
            this.checkBoxDisignation.Text = "Показать надписи/обозначения вида";
            this.checkBoxDisignation.UseVisualStyleBackColor = true;
            this.checkBoxDisignation.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxBreakLinesVisible
            // 
            this.checkBoxBreakLinesVisible.AutoSize = true;
            this.checkBoxBreakLinesVisible.Location = new System.Drawing.Point(142, 179);
            this.checkBoxBreakLinesVisible.Name = "checkBoxBreakLinesVisible";
            this.checkBoxBreakLinesVisible.Size = new System.Drawing.Size(158, 17);
            this.checkBoxBreakLinesVisible.TabIndex = 3;
            this.checkBoxBreakLinesVisible.Text = "Показать линии перехода";
            this.checkBoxBreakLinesVisible.UseVisualStyleBackColor = true;
            this.checkBoxBreakLinesVisible.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxBendsLinesVisible
            // 
            this.checkBoxBendsLinesVisible.AutoSize = true;
            this.checkBoxBendsLinesVisible.Location = new System.Drawing.Point(142, 156);
            this.checkBoxBendsLinesVisible.Name = "checkBoxBendsLinesVisible";
            this.checkBoxBendsLinesVisible.Size = new System.Drawing.Size(146, 17);
            this.checkBoxBendsLinesVisible.TabIndex = 3;
            this.checkBoxBendsLinesVisible.Text = "Показать линии сгибов";
            this.checkBoxBendsLinesVisible.UseVisualStyleBackColor = true;
            this.checkBoxBendsLinesVisible.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxCenterLinesVisible
            // 
            this.checkBoxCenterLinesVisible.AutoSize = true;
            this.checkBoxCenterLinesVisible.Location = new System.Drawing.Point(142, 133);
            this.checkBoxCenterLinesVisible.Name = "checkBoxCenterLinesVisible";
            this.checkBoxCenterLinesVisible.Size = new System.Drawing.Size(198, 17);
            this.checkBoxCenterLinesVisible.TabIndex = 3;
            this.checkBoxCenterLinesVisible.Text = "Показать осевые линии и центры";
            this.checkBoxCenterLinesVisible.UseVisualStyleBackColor = true;
            this.checkBoxCenterLinesVisible.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxBreakLink
            // 
            this.checkBoxBreakLink.AutoSize = true;
            this.checkBoxBreakLink.Checked = true;
            this.checkBoxBreakLink.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxBreakLink.Location = new System.Drawing.Point(142, 110);
            this.checkBoxBreakLink.Name = "checkBoxBreakLink";
            this.checkBoxBreakLink.Size = new System.Drawing.Size(173, 17);
            this.checkBoxBreakLink.TabIndex = 3;
            this.checkBoxBreakLink.Text = "Разрывать связь с моделью";
            this.checkBoxBreakLink.UseVisualStyleBackColor = true;
            this.checkBoxBreakLink.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // buttonPath
            // 
            this.buttonPath.Location = new System.Drawing.Point(620, 55);
            this.buttonPath.Name = "buttonPath";
            this.buttonPath.Size = new System.Drawing.Size(30, 23);
            this.buttonPath.TabIndex = 2;
            this.buttonPath.Text = "...";
            this.buttonPath.UseVisualStyleBackColor = true;
            this.buttonPath.Click += new System.EventHandler(this.buttonPath_Click);
            // 
            // labelPath
            // 
            this.labelPath.AutoSize = true;
            this.labelPath.Location = new System.Drawing.Point(139, 42);
            this.labelPath.Name = "labelPath";
            this.labelPath.Size = new System.Drawing.Size(149, 13);
            this.labelPath.TabIndex = 1;
            this.labelPath.Text = "Папка для сохранения DXF:";
            // 
            // textBoxPath
            // 
            this.textBoxPath.Location = new System.Drawing.Point(142, 58);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(472, 20);
            this.textBoxPath.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.checkBoxSimple);
            this.tabPage3.Controls.Add(this.buttonSaveSimple);
            this.tabPage3.Controls.Add(this.labelDescription);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.labelReplace);
            this.tabPage3.Controls.Add(this.labelSeparator);
            this.tabPage3.Controls.Add(this.labelNameSimple);
            this.tabPage3.Controls.Add(this.labelSample);
            this.tabPage3.Controls.Add(this.labelSave);
            this.tabPage3.Controls.Add(this.labelProperties);
            this.tabPage3.Controls.Add(this.listBoxSaveSimple);
            this.tabPage3.Controls.Add(this.textBoxReplaceOut);
            this.tabPage3.Controls.Add(this.textBoxReplaceIn);
            this.tabPage3.Controls.Add(this.listProperties);
            this.tabPage3.Controls.Add(this.textBoxNameSimple);
            this.tabPage3.Controls.Add(this.textSeparator);
            this.tabPage3.Controls.Add(this.textNameFile);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(792, 350);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "💾 Параметры имени файла";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // checkBoxSimple
            // 
            this.checkBoxSimple.AutoSize = true;
            this.checkBoxSimple.Location = new System.Drawing.Point(673, 44);
            this.checkBoxSimple.Name = "checkBoxSimple";
            this.checkBoxSimple.Size = new System.Drawing.Size(97, 17);
            this.checkBoxSimple.TabIndex = 7;
            this.checkBoxSimple.Text = "по умолчанию";
            this.checkBoxSimple.UseVisualStyleBackColor = true;
            // 
            // buttonSaveSimple
            // 
            this.buttonSaveSimple.Enabled = false;
            this.buttonSaveSimple.Location = new System.Drawing.Point(671, 60);
            this.buttonSaveSimple.Name = "buttonSaveSimple";
            this.buttonSaveSimple.Size = new System.Drawing.Size(113, 23);
            this.buttonSaveSimple.TabIndex = 6;
            this.buttonSaveSimple.Text = "Сохранить шаблон";
            this.buttonSaveSimple.UseVisualStyleBackColor = true;
            this.buttonSaveSimple.Click += new System.EventHandler(this.buttonSaveSimple_Click);
            // 
            // labelDescription
            // 
            this.labelDescription.AutoSize = true;
            this.labelDescription.Location = new System.Drawing.Point(153, 114);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(523, 195);
            this.labelDescription.TabIndex = 6;
            this.labelDescription.Text = resources.GetString("labelDescription.Text");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(317, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "=>";
            // 
            // labelReplace
            // 
            this.labelReplace.AutoSize = true;
            this.labelReplace.Location = new System.Drawing.Point(252, 51);
            this.labelReplace.Name = "labelReplace";
            this.labelReplace.Size = new System.Drawing.Size(49, 13);
            this.labelReplace.TabIndex = 3;
            this.labelReplace.Text = "Замена:";
            // 
            // labelSeparator
            // 
            this.labelSeparator.AutoSize = true;
            this.labelSeparator.Location = new System.Drawing.Point(153, 51);
            this.labelSeparator.Name = "labelSeparator";
            this.labelSeparator.Size = new System.Drawing.Size(76, 13);
            this.labelSeparator.TabIndex = 3;
            this.labelSeparator.Text = "Разделитель:";
            // 
            // labelNameSimple
            // 
            this.labelNameSimple.AutoSize = true;
            this.labelNameSimple.Location = new System.Drawing.Point(668, 7);
            this.labelNameSimple.Name = "labelNameSimple";
            this.labelNameSimple.Size = new System.Drawing.Size(49, 13);
            this.labelNameSimple.TabIndex = 3;
            this.labelNameSimple.Text = "Шаблон:";
            // 
            // labelSample
            // 
            this.labelSample.AutoSize = true;
            this.labelSample.Location = new System.Drawing.Point(153, 7);
            this.labelSample.Name = "labelSample";
            this.labelSample.Size = new System.Drawing.Size(49, 13);
            this.labelSample.TabIndex = 3;
            this.labelSample.Text = "Шаблон:";
            // 
            // labelSave
            // 
            this.labelSave.AutoSize = true;
            this.labelSave.Location = new System.Drawing.Point(9, 176);
            this.labelSave.Name = "labelSave";
            this.labelSave.Size = new System.Drawing.Size(127, 13);
            this.labelSave.TabIndex = 2;
            this.labelSave.Text = "Сохраненные шаблоны:";
            // 
            // labelProperties
            // 
            this.labelProperties.AutoSize = true;
            this.labelProperties.Location = new System.Drawing.Point(9, 7);
            this.labelProperties.Name = "labelProperties";
            this.labelProperties.Size = new System.Drawing.Size(110, 13);
            this.labelProperties.TabIndex = 2;
            this.labelProperties.Text = "Свойства элемента:";
            // 
            // listBoxSaveSimple
            // 
            this.listBoxSaveSimple.ContextMenuStrip = this.contextMenuTemplates;
            this.listBoxSaveSimple.FormattingEnabled = true;
            this.listBoxSaveSimple.Items.AddRange(new object[] {
            "Пусто"});
            this.listBoxSaveSimple.Location = new System.Drawing.Point(12, 192);
            this.listBoxSaveSimple.Name = "listBoxSaveSimple";
            this.listBoxSaveSimple.Size = new System.Drawing.Size(120, 147);
            this.listBoxSaveSimple.TabIndex = 0;
            this.listBoxSaveSimple.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxSaveSimple_DrawItem);
            this.listBoxSaveSimple.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxSaveSimple_MouseDoubleClick);
            this.listBoxSaveSimple.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBoxSaveSimple_MouseDown);
            this.listBoxSaveSimple.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            // 
            // contextMenuTemplates
            // 
            this.contextMenuTemplates.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSetDefault,
            this.menuDeleteTemplate});
            this.contextMenuTemplates.Name = "contextMenuTemplates";
            this.contextMenuTemplates.Size = new System.Drawing.Size(238, 48);
            // 
            // menuSetDefault
            // 
            this.menuSetDefault.Name = "menuSetDefault";
            this.menuSetDefault.Size = new System.Drawing.Size(237, 22);
            this.menuSetDefault.Text = "Использовать по умолчанию";
            this.menuSetDefault.Click += new System.EventHandler(this.menuSetDefault_Click);
            // 
            // menuDeleteTemplate
            // 
            this.menuDeleteTemplate.Name = "menuDeleteTemplate";
            this.menuDeleteTemplate.Size = new System.Drawing.Size(237, 22);
            this.menuDeleteTemplate.Text = "Удалить шаблон";
            this.menuDeleteTemplate.Click += new System.EventHandler(this.menuDeleteTemplate_Click);
            // 
            // textBoxReplaceOut
            // 
            this.textBoxReplaceOut.Location = new System.Drawing.Point(339, 67);
            this.textBoxReplaceOut.Name = "textBoxReplaceOut";
            this.textBoxReplaceOut.Size = new System.Drawing.Size(60, 20);
            this.textBoxReplaceOut.TabIndex = 0;
            // 
            // textBoxReplaceIn
            // 
            this.textBoxReplaceIn.Location = new System.Drawing.Point(255, 67);
            this.textBoxReplaceIn.Name = "textBoxReplaceIn";
            this.textBoxReplaceIn.Size = new System.Drawing.Size(60, 20);
            this.textBoxReplaceIn.TabIndex = 0;
            // 
            // listProperties
            // 
            this.listProperties.FormattingEnabled = true;
            this.listProperties.Items.AddRange(new object[] {
            "{ИмяФайлаОриг}",
            "{Имя}",
            "{Обазначение}",
            "{Толщина металла}",
            "{Развертка}",
            "{Вид}",
            "{День}",
            "{Месяц}",
            "{Год}",
            "{Номер}",
            "{Разделитель}"});
            this.listProperties.Location = new System.Drawing.Point(12, 23);
            this.listProperties.Name = "listProperties";
            this.listProperties.Size = new System.Drawing.Size(120, 147);
            this.listProperties.TabIndex = 0;
            this.listProperties.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listProperties_MouseDoubleClick);
            // 
            // textBoxNameSimple
            // 
            this.textBoxNameSimple.Location = new System.Drawing.Point(671, 23);
            this.textBoxNameSimple.Name = "textBoxNameSimple";
            this.textBoxNameSimple.Size = new System.Drawing.Size(113, 20);
            this.textBoxNameSimple.TabIndex = 0;
            this.textBoxNameSimple.TextChanged += new System.EventHandler(this.textBoxNameSimple_TextChanged);
            // 
            // textSeparator
            // 
            this.textSeparator.Location = new System.Drawing.Point(156, 67);
            this.textSeparator.Name = "textSeparator";
            this.textSeparator.Size = new System.Drawing.Size(46, 20);
            this.textSeparator.TabIndex = 0;
            this.textSeparator.Text = " _ ";
            // 
            // textNameFile
            // 
            this.textNameFile.Location = new System.Drawing.Point(156, 23);
            this.textNameFile.Name = "textNameFile";
            this.textNameFile.Size = new System.Drawing.Size(487, 20);
            this.textNameFile.TabIndex = 0;
            this.textNameFile.Text = "{ИмяФайлаОриг}";
            // 
            // previewPart
            // 
            this.previewPart.AutoSize = true;
            this.previewPart.Location = new System.Drawing.Point(1, 412);
            this.previewPart.Name = "previewPart";
            this.previewPart.Size = new System.Drawing.Size(10, 13);
            this.previewPart.TabIndex = 5;
            this.previewPart.Text = "-";
            // 
            // выводDXFToolStripMenuItem
            // 
            this.выводDXFToolStripMenuItem.Name = "выводDXFToolStripMenuItem";
            this.выводDXFToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // button_Export
            // 
            this.button_Export.ContextMenuStrip = this.contextMenuExport;
            this.button_Export.Enabled = false;
            this.button_Export.Location = new System.Drawing.Point(702, 11);
            this.button_Export.Name = "button_Export";
            this.button_Export.Size = new System.Drawing.Size(86, 23);
            this.button_Export.TabIndex = 6;
            this.button_Export.Text = "Вывести DXF";
            this.button_Export.UseVisualStyleBackColor = true;
            this.button_Export.Click += new System.EventHandler(this.button_Export_Click);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button_Export);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.previewPart);
            this.Controls.Add(this.statusStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "ExportDXF - Компас";
            this.contextMenuExport.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.contextMenuTemplates.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeParts;
        private System.Windows.Forms.Button butScan;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Label label_type;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStripMenuItem выводDXFToolStripMenuItem;
        private System.Windows.Forms.Button button_Export;
        private System.Windows.Forms.CheckBox flagExportDXF;
        private System.Windows.Forms.Label label_marking;
        private System.Windows.Forms.Label label_name;
        private System.Windows.Forms.Label label_count;
        private System.Windows.Forms.Label label_thickness;
        private System.Windows.Forms.Label label_bendCount;
        private System.Windows.Forms.Label label_view;
        private System.Windows.Forms.Label text_bendCount;
        private System.Windows.Forms.Label text_thickness;
        private System.Windows.Forms.Label text_count;
        private System.Windows.Forms.Label text_name;
        private System.Windows.Forms.Label text_marking;
        private System.Windows.Forms.Label text_type;
        private System.Windows.Forms.ComboBox combo_view;
        private System.Windows.Forms.Label label_Info;
        private System.Windows.Forms.Button buttonRef_view;
        private System.Windows.Forms.Label labelPath;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.CheckBox checkBoxCreateViewElements;
        private System.Windows.Forms.CheckBox checkBoxDisignation;
        private System.Windows.Forms.CheckBox checkBoxBreakLinesVisible;
        private System.Windows.Forms.CheckBox checkBoxBendsLinesVisible;
        private System.Windows.Forms.CheckBox checkBoxCenterLinesVisible;
        private System.Windows.Forms.CheckBox checkBoxBreakLink;
        private System.Windows.Forms.Button buttonPath;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button buttonOpenPart;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox textNameFile;
        private System.Windows.Forms.Label previewPart;
        private System.Windows.Forms.Label labelSample;
        private System.Windows.Forms.Label labelProperties;
        private System.Windows.Forms.ListBox listProperties;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Label labelSeparator;
        private System.Windows.Forms.TextBox textSeparator;
        private System.Windows.Forms.Button buttonSaveSimple;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelReplace;
        private System.Windows.Forms.Label labelSave;
        private System.Windows.Forms.ListBox listBoxSaveSimple;
        private System.Windows.Forms.TextBox textBoxReplaceOut;
        private System.Windows.Forms.TextBox textBoxReplaceIn;
        private System.Windows.Forms.CheckBox checkBoxSimple;
        private System.Windows.Forms.Label labelNameSimple;
        private System.Windows.Forms.TextBox textBoxNameSimple;
        private System.Windows.Forms.ContextMenuStrip contextMenuTemplates;
        private System.Windows.Forms.ToolStripMenuItem menuSetDefault;
        private System.Windows.Forms.ToolStripMenuItem menuDeleteTemplate;
        private System.Windows.Forms.ContextMenuStrip contextMenuExport;
        private System.Windows.Forms.ToolStripMenuItem выводDXFПоОригинальномуИмениToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem выводToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem пустоToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripInfoLabel;
        private System.Windows.Forms.CheckBox checkBoxRemoveOuterDiameter;
    }
}