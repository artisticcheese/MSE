using System.Windows.Forms;
namespace Microsoft.Mse.Gui
{
	partial class MainGui
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>

        // Our refresh intervals could be less than a second
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges")]
        private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainGui));
            this.ptSplitInfo = new System.Windows.Forms.SplitContainer();
            this.processSplitThread = new System.Windows.Forms.SplitContainer();
            this.processView = new System.Windows.Forms.ListView();
            this.ProcessName = new System.Windows.Forms.ColumnHeader();
            this.PID = new System.Windows.Forms.ColumnHeader();
            this.processViewMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewThreadsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setPriorityMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.realTimeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.highMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboveNormalItem = new System.Windows.Forms.ToolStripMenuItem();
            this.normalItem = new System.Windows.Forms.ToolStripMenuItem();
            this.belowNormalItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kilMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iconList = new System.Windows.Forms.ImageList(this.components);
            this.threadView = new System.Windows.Forms.ListView();
            this.tid = new System.Windows.Forms.ColumnHeader();
            this.threadState = new System.Windows.Forms.ColumnHeader();
            this.threadViewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewStackTraceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeInfoMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.processDataLayout = new System.Windows.Forms.TableLayoutPanel();
            this.label12 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.listsSplitData = new System.Windows.Forms.SplitContainer();
            this.dataBox = new System.Windows.Forms.RichTextBox();
            this.dataBoxContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.takeStackSnapShotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.keepontopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoRefreshThreadListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoRefreshStackTraceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewStackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startMonitoringStackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goToMSEHomepageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gotoMSEForumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusBarProcesses = new System.Windows.Forms.ToolStripStatusLabel();
            this.processViewMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.processRefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.collapseDataViewButton = new System.Windows.Forms.Button();
            this.threadRefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.logFileSaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.stackRefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.stackLogTimer = new System.Windows.Forms.Timer(this.components);
            this.monitorFileSaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.dataRefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.toolBarContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageandTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainToolBar = new System.Windows.Forms.ToolStrip();
            this.toolThreadViewRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStackRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolViewStack = new System.Windows.Forms.ToolStripButton();
            this.toolSaveStack = new System.Windows.Forms.ToolStripButton();
            this.toolMonitorStack = new System.Windows.Forms.ToolStripButton();
            this.findButton = new System.Windows.Forms.ToolStripButton();
            this.collapseButtonHolder = new System.Windows.Forms.Panel();
            this.ptSplitInfo.Panel1.SuspendLayout();
            this.ptSplitInfo.Panel2.SuspendLayout();
            this.ptSplitInfo.SuspendLayout();
            this.processSplitThread.Panel1.SuspendLayout();
            this.processSplitThread.Panel2.SuspendLayout();
            this.processSplitThread.SuspendLayout();
            this.processViewMenu.SuspendLayout();
            this.threadViewContextMenu.SuspendLayout();
            this.changeInfoMenu.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.processDataLayout.SuspendLayout();
            this.listsSplitData.Panel1.SuspendLayout();
            this.listsSplitData.Panel2.SuspendLayout();
            this.listsSplitData.SuspendLayout();
            this.dataBoxContextMenu.SuspendLayout();
            this.mainMenu.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolBarContextMenu.SuspendLayout();
            this.mainToolBar.SuspendLayout();
            this.collapseButtonHolder.SuspendLayout();
            this.SuspendLayout();
            // 
            // ptSplitInfo
            // 
            this.ptSplitInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.ptSplitInfo, "ptSplitInfo");
            this.ptSplitInfo.Name = "ptSplitInfo";
            // 
            // ptSplitInfo.Panel1
            // 
            this.ptSplitInfo.Panel1.Controls.Add(this.processSplitThread);
            // 
            // ptSplitInfo.Panel2
            // 
            this.ptSplitInfo.Panel2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ptSplitInfo.Panel2.ContextMenuStrip = this.changeInfoMenu;
            this.ptSplitInfo.Panel2.Controls.Add(this.groupBox2);
            resources.ApplyResources(this.ptSplitInfo.Panel2, "ptSplitInfo.Panel2");
            this.ptSplitInfo.TabStop = false;
            // 
            // processSplitThread
            // 
            this.processSplitThread.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.processSplitThread, "processSplitThread");
            this.processSplitThread.Name = "processSplitThread";
            // 
            // processSplitThread.Panel1
            // 
            this.processSplitThread.Panel1.Controls.Add(this.processView);
            // 
            // processSplitThread.Panel2
            // 
            this.processSplitThread.Panel2.Controls.Add(this.threadView);
            this.processSplitThread.TabStop = false;
            // 
            // processView
            // 
            this.processView.AllowColumnReorder = true;
            this.processView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ProcessName,
            this.PID});
            this.processView.ContextMenuStrip = this.processViewMenu;
            resources.ApplyResources(this.processView, "processView");
            this.processView.FullRowSelect = true;
            this.processView.HideSelection = false;
            this.processView.MultiSelect = false;
            this.processView.Name = "processView";
            this.processView.ShowItemToolTips = true;
            this.processView.SmallImageList = this.iconList;
            this.processView.UseCompatibleStateImageBehavior = false;
            this.processView.View = System.Windows.Forms.View.Details;
            this.processView.DoubleClick += new System.EventHandler(this.processView_DoubleClick);
            this.processView.SelectedIndexChanged += new System.EventHandler(this.processView_SelectedIndexChanged);
            this.processView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.processView_KeyDown);
            this.processView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.processView_ColumnClick);
            // 
            // ProcessName
            // 
            resources.ApplyResources(this.ProcessName, "ProcessName");
            // 
            // PID
            // 
            resources.ApplyResources(this.PID, "PID");
            // 
            // processViewMenu
            // 
            this.processViewMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewThreadsToolStripMenuItem,
            this.setPriorityMenuItem,
            this.kilMenuItem});
            this.processViewMenu.Name = "processViewMenu";
            resources.ApplyResources(this.processViewMenu, "processViewMenu");
            // 
            // viewThreadsToolStripMenuItem
            // 
            resources.ApplyResources(this.viewThreadsToolStripMenuItem, "viewThreadsToolStripMenuItem");
            this.viewThreadsToolStripMenuItem.Name = "viewThreadsToolStripMenuItem";
            this.viewThreadsToolStripMenuItem.Click += new System.EventHandler(this.viewThreadsToolStripMenuItem_Click);
            // 
            // setPriorityMenuItem
            // 
            this.setPriorityMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.realTimeMenuItem,
            this.highMenuItem,
            this.aboveNormalItem,
            this.normalItem,
            this.belowNormalItem,
            this.lowMenuItem});
            this.setPriorityMenuItem.Name = "setPriorityMenuItem";
            resources.ApplyResources(this.setPriorityMenuItem, "setPriorityMenuItem");
            this.setPriorityMenuItem.DropDownOpened += new System.EventHandler(this.setPriorityMenuItem_DropDownOpened);
            // 
            // realTimeMenuItem
            // 
            this.realTimeMenuItem.Name = "realTimeMenuItem";
            resources.ApplyResources(this.realTimeMenuItem, "realTimeMenuItem");
            this.realTimeMenuItem.Click += new System.EventHandler(this.realTimeMenuItem_Click);
            // 
            // highMenuItem
            // 
            this.highMenuItem.Name = "highMenuItem";
            resources.ApplyResources(this.highMenuItem, "highMenuItem");
            this.highMenuItem.Click += new System.EventHandler(this.highMenuItem_Click);
            // 
            // aboveNormalItem
            // 
            this.aboveNormalItem.Name = "aboveNormalItem";
            resources.ApplyResources(this.aboveNormalItem, "aboveNormalItem");
            this.aboveNormalItem.Click += new System.EventHandler(this.aboveNormalItem_Click);
            // 
            // normalItem
            // 
            this.normalItem.Name = "normalItem";
            resources.ApplyResources(this.normalItem, "normalItem");
            this.normalItem.Click += new System.EventHandler(this.normalItem_Click);
            // 
            // belowNormalItem
            // 
            this.belowNormalItem.Name = "belowNormalItem";
            resources.ApplyResources(this.belowNormalItem, "belowNormalItem");
            this.belowNormalItem.Click += new System.EventHandler(this.belowNormalItem_Click);
            // 
            // lowMenuItem
            // 
            this.lowMenuItem.Name = "lowMenuItem";
            resources.ApplyResources(this.lowMenuItem, "lowMenuItem");
            this.lowMenuItem.Click += new System.EventHandler(this.lowMenuItem_Click);
            // 
            // kilMenuItem
            // 
            this.kilMenuItem.Name = "kilMenuItem";
            resources.ApplyResources(this.kilMenuItem, "kilMenuItem");
            this.kilMenuItem.Click += new System.EventHandler(this.killMenuItem_Click);
            // 
            // iconList
            // 
            this.iconList.ColorDepth = System.Windows.Forms.ColorDepth.Depth16Bit;
            resources.ApplyResources(this.iconList, "iconList");
            this.iconList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // threadView
            // 
            this.threadView.AllowColumnReorder = true;
            this.threadView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.tid,
            this.threadState});
            this.threadView.ContextMenuStrip = this.threadViewContextMenu;
            resources.ApplyResources(this.threadView, "threadView");
            this.threadView.FullRowSelect = true;
            this.threadView.HideSelection = false;
            this.threadView.Name = "threadView";
            this.threadView.ShowItemToolTips = true;
            this.threadView.UseCompatibleStateImageBehavior = false;
            this.threadView.View = System.Windows.Forms.View.Details;
            this.threadView.DoubleClick += new System.EventHandler(this.threadView_DoubleClick);
            this.threadView.SelectedIndexChanged += new System.EventHandler(this.threadView_SelectedIndexChanged);
            this.threadView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.threadView_KeyDown);
            this.threadView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.threadView_ColumnClick);
            // 
            // tid
            // 
            resources.ApplyResources(this.tid, "tid");
            // 
            // threadState
            // 
            resources.ApplyResources(this.threadState, "threadState");
            // 
            // threadViewContextMenu
            // 
            this.threadViewContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewStackTraceToolStripMenuItem});
            this.threadViewContextMenu.Name = "threadViewContextMenu";
            resources.ApplyResources(this.threadViewContextMenu, "threadViewContextMenu");
            // 
            // viewStackTraceToolStripMenuItem
            // 
            resources.ApplyResources(this.viewStackTraceToolStripMenuItem, "viewStackTraceToolStripMenuItem");
            this.viewStackTraceToolStripMenuItem.Name = "viewStackTraceToolStripMenuItem";
            this.viewStackTraceToolStripMenuItem.Click += new System.EventHandler(this.viewStackTraceToolStripMenuItem_Click);
            // 
            // changeInfoMenu
            // 
            this.changeInfoMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeToolStripMenuItem});
            this.changeInfoMenu.Name = "changeInfoMenu";
            resources.ApplyResources(this.changeInfoMenu, "changeInfoMenu");
            // 
            // changeToolStripMenuItem
            // 
            this.changeToolStripMenuItem.Name = "changeToolStripMenuItem";
            resources.ApplyResources(this.changeToolStripMenuItem, "changeToolStripMenuItem");
            this.changeToolStripMenuItem.Click += new System.EventHandler(this.changeToolStripMenuItem_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.groupBox2.Controls.Add(this.processDataLayout);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox2.ForeColor = System.Drawing.SystemColors.InfoText;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // processDataLayout
            // 
            resources.ApplyResources(this.processDataLayout, "processDataLayout");
            this.processDataLayout.Controls.Add(this.label12, 1, 5);
            this.processDataLayout.Controls.Add(this.label4, 1, 1);
            this.processDataLayout.Controls.Add(this.label3, 0, 1);
            this.processDataLayout.Controls.Add(this.label2, 1, 0);
            this.processDataLayout.Controls.Add(this.label1, 0, 0);
            this.processDataLayout.Controls.Add(this.label10, 1, 4);
            this.processDataLayout.Controls.Add(this.label9, 0, 4);
            this.processDataLayout.Controls.Add(this.label8, 1, 3);
            this.processDataLayout.Controls.Add(this.label7, 0, 3);
            this.processDataLayout.Controls.Add(this.label5, 0, 2);
            this.processDataLayout.Controls.Add(this.label11, 0, 5);
            this.processDataLayout.Controls.Add(this.label6, 1, 2);
            this.processDataLayout.Name = "processDataLayout";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // listsSplitData
            // 
            resources.ApplyResources(this.listsSplitData, "listsSplitData");
            this.listsSplitData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listsSplitData.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.listsSplitData.Name = "listsSplitData";
            // 
            // listsSplitData.Panel1
            // 
            this.listsSplitData.Panel1.Controls.Add(this.ptSplitInfo);
            // 
            // listsSplitData.Panel2
            // 
            this.listsSplitData.Panel2.Controls.Add(this.dataBox);
            this.listsSplitData.Panel2Collapsed = true;
            this.listsSplitData.TabStop = false;
            // 
            // dataBox
            // 
            this.dataBox.ContextMenuStrip = this.dataBoxContextMenu;
            this.dataBox.DetectUrls = false;
            resources.ApplyResources(this.dataBox, "dataBox");
            this.dataBox.HideSelection = false;
            this.dataBox.Name = "dataBox";
            this.dataBox.ReadOnly = true;
            this.dataBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataBox_KeyDown);
            // 
            // dataBoxContextMenu
            // 
            this.dataBoxContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.findToolStripMenuItem});
            this.dataBoxContextMenu.Name = "dataBoxContextMenu";
            resources.ApplyResources(this.dataBoxContextMenu, "dataBoxContextMenu");
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            resources.ApplyResources(this.selectAllToolStripMenuItem, "selectAllToolStripMenuItem");
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            resources.ApplyResources(this.findToolStripMenuItem, "findToolStripMenuItem");
            this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.actionToolStripMenuItem,
            this.helpToolStripMenuItem});
            resources.ApplyResources(this.mainMenu, "mainMenu");
            this.mainMenu.Name = "mainMenu";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.takeStackSnapShotToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // takeStackSnapShotToolStripMenuItem
            // 
            resources.ApplyResources(this.takeStackSnapShotToolStripMenuItem, "takeStackSnapShotToolStripMenuItem");
            this.takeStackSnapShotToolStripMenuItem.Name = "takeStackSnapShotToolStripMenuItem";
            this.takeStackSnapShotToolStripMenuItem.Click += new System.EventHandler(this.toolStackSnapShot_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.keepontopToolStripMenuItem,
            this.preferencesToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            resources.ApplyResources(this.optionsToolStripMenuItem, "optionsToolStripMenuItem");
            // 
            // keepontopToolStripMenuItem
            // 
            this.keepontopToolStripMenuItem.CheckOnClick = true;
            this.keepontopToolStripMenuItem.Name = "keepontopToolStripMenuItem";
            resources.ApplyResources(this.keepontopToolStripMenuItem, "keepontopToolStripMenuItem");
            this.keepontopToolStripMenuItem.Click += new System.EventHandler(this.keepontopToolStripMenuItem_Click);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            resources.ApplyResources(this.preferencesToolStripMenuItem, "preferencesToolStripMenuItem");
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
            // 
            // actionToolStripMenuItem
            // 
            this.actionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoRefreshThreadListToolStripMenuItem,
            this.autoRefreshStackTraceToolStripMenuItem,
            this.viewStackToolStripMenuItem,
            this.startMonitoringStackToolStripMenuItem,
            this.findToolStripMenuItem1});
            this.actionToolStripMenuItem.Name = "actionToolStripMenuItem";
            resources.ApplyResources(this.actionToolStripMenuItem, "actionToolStripMenuItem");
            // 
            // autoRefreshThreadListToolStripMenuItem
            // 
            this.autoRefreshThreadListToolStripMenuItem.Name = "autoRefreshThreadListToolStripMenuItem";
            resources.ApplyResources(this.autoRefreshThreadListToolStripMenuItem, "autoRefreshThreadListToolStripMenuItem");
            this.autoRefreshThreadListToolStripMenuItem.Click += new System.EventHandler(this.toolThreadViewRefresh_Click);
            // 
            // autoRefreshStackTraceToolStripMenuItem
            // 
            this.autoRefreshStackTraceToolStripMenuItem.Name = "autoRefreshStackTraceToolStripMenuItem";
            resources.ApplyResources(this.autoRefreshStackTraceToolStripMenuItem, "autoRefreshStackTraceToolStripMenuItem");
            this.autoRefreshStackTraceToolStripMenuItem.Click += new System.EventHandler(this.toolStackRefresh_Click);
            // 
            // viewStackToolStripMenuItem
            // 
            resources.ApplyResources(this.viewStackToolStripMenuItem, "viewStackToolStripMenuItem");
            this.viewStackToolStripMenuItem.Name = "viewStackToolStripMenuItem";
            this.viewStackToolStripMenuItem.Click += new System.EventHandler(this.viewStackToolStripMenuItem_Click);
            // 
            // startMonitoringStackToolStripMenuItem
            // 
            resources.ApplyResources(this.startMonitoringStackToolStripMenuItem, "startMonitoringStackToolStripMenuItem");
            this.startMonitoringStackToolStripMenuItem.Name = "startMonitoringStackToolStripMenuItem";
            this.startMonitoringStackToolStripMenuItem.Click += new System.EventHandler(this.toolMonitorStack_Click);
            // 
            // findToolStripMenuItem1
            // 
            resources.ApplyResources(this.findToolStripMenuItem1, "findToolStripMenuItem1");
            this.findToolStripMenuItem1.Name = "findToolStripMenuItem1";
            this.findToolStripMenuItem1.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.goToMSEHomepageToolStripMenuItem,
            this.gotoMSEForumToolStripMenuItem,
            this.toolStripSeparator2,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // goToMSEHomepageToolStripMenuItem
            // 
            this.goToMSEHomepageToolStripMenuItem.Name = "goToMSEHomepageToolStripMenuItem";
            resources.ApplyResources(this.goToMSEHomepageToolStripMenuItem, "goToMSEHomepageToolStripMenuItem");
            this.goToMSEHomepageToolStripMenuItem.Click += new System.EventHandler(this.goToMSEHomepageToolStripMenuItem_Click);
            // 
            // gotoMSEForumToolStripMenuItem
            // 
            this.gotoMSEForumToolStripMenuItem.Name = "gotoMSEForumToolStripMenuItem";
            resources.ApplyResources(this.gotoMSEForumToolStripMenuItem, "gotoMSEForumToolStripMenuItem");
            this.gotoMSEForumToolStripMenuItem.Click += new System.EventHandler(this.gotoMSEForumToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBarProcesses});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // statusBarProcesses
            // 
            this.statusBarProcesses.Name = "statusBarProcesses";
            resources.ApplyResources(this.statusBarProcesses, "statusBarProcesses");
            // 
            // processViewMenuItem1
            // 
            this.processViewMenuItem1.Name = "processViewMenuItem1";
            resources.ApplyResources(this.processViewMenuItem1, "processViewMenuItem1");
            // 
            // processRefreshTimer
            // 
            this.processRefreshTimer.Enabled = true;
            this.processRefreshTimer.Interval = 250;
            this.processRefreshTimer.Tick += new System.EventHandler(this.processRefreshTimer_Tick);
            // 
            // collapseDataViewButton
            // 
            this.collapseDataViewButton.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            resources.ApplyResources(this.collapseDataViewButton, "collapseDataViewButton");
            this.collapseDataViewButton.FlatAppearance.BorderSize = 0;
            this.collapseDataViewButton.Name = "collapseDataViewButton";
            this.collapseDataViewButton.TabStop = false;
            this.collapseDataViewButton.UseVisualStyleBackColor = false;
            this.collapseDataViewButton.Click += new System.EventHandler(this.collapseDataViewButton_Click);
            // 
            // threadRefreshTimer
            // 
            this.threadRefreshTimer.Enabled = true;
            this.threadRefreshTimer.Interval = 1000;
            this.threadRefreshTimer.Tick += new System.EventHandler(this.threadRefreshTimer_Tick);
            // 
            // logFileSaveDialog
            // 
            this.logFileSaveDialog.DefaultExt = "log";
            resources.ApplyResources(this.logFileSaveDialog, "logFileSaveDialog");
            // 
            // stackRefreshTimer
            // 
            this.stackRefreshTimer.Interval = 5000;
            this.stackRefreshTimer.Tick += new System.EventHandler(this.stackRefreshTimer_Tick);
            // 
            // stackLogTimer
            // 
            this.stackLogTimer.Interval = 5000;
            this.stackLogTimer.Tick += new System.EventHandler(this.stackLogTimer_Tick);
            // 
            // monitorFileSaveDialog
            // 
            this.monitorFileSaveDialog.DefaultExt = "log";
            resources.ApplyResources(this.monitorFileSaveDialog, "monitorFileSaveDialog");
            // 
            // dataRefreshTimer
            // 
            this.dataRefreshTimer.Enabled = true;
            this.dataRefreshTimer.Interval = 1000;
            this.dataRefreshTimer.Tick += new System.EventHandler(this.dataRefreshTimer_Tick);
            // 
            // toolBarContextMenu
            // 
            this.toolBarContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem});
            this.toolBarContextMenu.Name = "toolBarContextMenu";
            resources.ApplyResources(this.toolBarContextMenu, "toolBarContextMenu");
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.imageToolStripMenuItem,
            this.imageandTextToolStripMenuItem,
            this.textToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            // 
            // imageToolStripMenuItem
            // 
            this.imageToolStripMenuItem.Checked = true;
            this.imageToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.imageToolStripMenuItem.Name = "imageToolStripMenuItem";
            resources.ApplyResources(this.imageToolStripMenuItem, "imageToolStripMenuItem");
            this.imageToolStripMenuItem.Click += new System.EventHandler(this.imageToolStripMenuItem_Click);
            // 
            // imageandTextToolStripMenuItem
            // 
            this.imageandTextToolStripMenuItem.Name = "imageandTextToolStripMenuItem";
            resources.ApplyResources(this.imageandTextToolStripMenuItem, "imageandTextToolStripMenuItem");
            this.imageandTextToolStripMenuItem.Click += new System.EventHandler(this.imageandTextToolStripMenuItem_Click);
            // 
            // textToolStripMenuItem
            // 
            this.textToolStripMenuItem.Name = "textToolStripMenuItem";
            resources.ApplyResources(this.textToolStripMenuItem, "textToolStripMenuItem");
            this.textToolStripMenuItem.Click += new System.EventHandler(this.textToolStripMenuItem_Click);
            // 
            // mainToolBar
            // 
            this.mainToolBar.ContextMenuStrip = this.toolBarContextMenu;
            this.mainToolBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.mainToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolThreadViewRefresh,
            this.toolStackRefresh,
            this.toolStripSeparator1,
            this.toolViewStack,
            this.toolSaveStack,
            this.toolMonitorStack,
            this.findButton});
            this.mainToolBar.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            resources.ApplyResources(this.mainToolBar, "mainToolBar");
            this.mainToolBar.Name = "mainToolBar";
            this.mainToolBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.mainToolBar.Stretch = true;
            // 
            // toolThreadViewRefresh
            // 
            this.toolThreadViewRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolThreadViewRefresh.Image = global::Microsoft.Mse.Gui.Properties.Resources.RefreshDocView;
            resources.ApplyResources(this.toolThreadViewRefresh, "toolThreadViewRefresh");
            this.toolThreadViewRefresh.Name = "toolThreadViewRefresh";
            this.toolThreadViewRefresh.Click += new System.EventHandler(this.toolThreadViewRefresh_Click);
            // 
            // toolStackRefresh
            // 
            this.toolStackRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStackRefresh.Image = global::Microsoft.Mse.Gui.Properties.Resources.Refresh;
            resources.ApplyResources(this.toolStackRefresh, "toolStackRefresh");
            this.toolStackRefresh.Name = "toolStackRefresh";
            this.toolStackRefresh.Click += new System.EventHandler(this.toolStackRefresh_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolViewStack
            // 
            this.toolViewStack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolViewStack, "toolViewStack");
            this.toolViewStack.Image = global::Microsoft.Mse.Gui.Properties.Resources.ShowAllComments;
            this.toolViewStack.Name = "toolViewStack";
            this.toolViewStack.Click += new System.EventHandler(this.toolViewStack_Click);
            // 
            // toolSaveStack
            // 
            this.toolSaveStack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolSaveStack, "toolSaveStack");
            this.toolSaveStack.Image = global::Microsoft.Mse.Gui.Properties.Resources.Save;
            this.toolSaveStack.Name = "toolSaveStack";
            this.toolSaveStack.Click += new System.EventHandler(this.toolStackSnapShot_Click);
            // 
            // toolMonitorStack
            // 
            this.toolMonitorStack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolMonitorStack, "toolMonitorStack");
            this.toolMonitorStack.Image = global::Microsoft.Mse.Gui.Properties.Resources.FormRun;
            this.toolMonitorStack.Name = "toolMonitorStack";
            this.toolMonitorStack.Click += new System.EventHandler(this.toolMonitorStack_Click);
            // 
            // findButton
            // 
            this.findButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.findButton, "findButton");
            this.findButton.Image = global::Microsoft.Mse.Gui.Properties.Resources.Search;
            this.findButton.Name = "findButton";
            this.findButton.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // collapseButtonHolder
            // 
            this.collapseButtonHolder.Controls.Add(this.collapseDataViewButton);
            resources.ApplyResources(this.collapseButtonHolder, "collapseButtonHolder");
            this.collapseButtonHolder.Name = "collapseButtonHolder";
            // 
            // MainGui
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.Controls.Add(this.collapseButtonHolder);
            this.Controls.Add(this.listsSplitData);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.mainToolBar);
            this.Controls.Add(this.mainMenu);
            this.DoubleBuffered = true;
            this.Name = "MainGui";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.mainGui_FormClosing);
            this.ptSplitInfo.Panel1.ResumeLayout(false);
            this.ptSplitInfo.Panel2.ResumeLayout(false);
            this.ptSplitInfo.ResumeLayout(false);
            this.processSplitThread.Panel1.ResumeLayout(false);
            this.processSplitThread.Panel2.ResumeLayout(false);
            this.processSplitThread.ResumeLayout(false);
            this.processViewMenu.ResumeLayout(false);
            this.threadViewContextMenu.ResumeLayout(false);
            this.changeInfoMenu.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.processDataLayout.ResumeLayout(false);
            this.processDataLayout.PerformLayout();
            this.listsSplitData.Panel1.ResumeLayout(false);
            this.listsSplitData.Panel2.ResumeLayout(false);
            this.listsSplitData.ResumeLayout(false);
            this.dataBoxContextMenu.ResumeLayout(false);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolBarContextMenu.ResumeLayout(false);
            this.mainToolBar.ResumeLayout(false);
            this.mainToolBar.PerformLayout();
            this.collapseButtonHolder.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		void mainGui_FormClosing(object sender, FormClosingEventArgs e)
		{
			saveSettignsCheck();
		}




		#endregion

		private System.Windows.Forms.SplitContainer listsSplitData;
		private System.Windows.Forms.SplitContainer processSplitThread;
		private System.Windows.Forms.SplitContainer ptSplitInfo;
		private System.Windows.Forms.ListView processView;
		private System.Windows.Forms.ColumnHeader ProcessName;
		private System.Windows.Forms.ColumnHeader PID;
		private System.Windows.Forms.ListView threadView;
		private System.Windows.Forms.ColumnHeader tid;
		private System.Windows.Forms.ColumnHeader threadState;



		//naviagtion form stuff
		private System.Windows.Forms.MenuStrip mainMenu;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel statusBarProcesses;
		private System.Windows.Forms.ContextMenuStrip processViewMenu;
		private System.Windows.Forms.ToolStripMenuItem processViewMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem kilMenuItem;
		private System.Windows.Forms.ToolStripMenuItem setPriorityMenuItem;
		private System.Windows.Forms.ToolStripMenuItem realTimeMenuItem;
		private System.Windows.Forms.ToolStripMenuItem highMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboveNormalItem;
		private System.Windows.Forms.ToolStripMenuItem normalItem;
		private System.Windows.Forms.ToolStripMenuItem belowNormalItem;
		private System.Windows.Forms.ToolStripMenuItem lowMenuItem;
		private System.Windows.Forms.ImageList iconList;
		private System.Windows.Forms.ToolStripMenuItem keepontopToolStripMenuItem;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Timer processRefreshTimer;
        private System.Windows.Forms.Button collapseDataViewButton;
		private System.Windows.Forms.Timer threadRefreshTimer;
		private System.Windows.Forms.TableLayoutPanel processDataLayout;
		private System.Windows.Forms.SaveFileDialog logFileSaveDialog;
		private System.Windows.Forms.Timer stackRefreshTimer;
		private System.Windows.Forms.Timer stackLogTimer;
		private System.Windows.Forms.SaveFileDialog monitorFileSaveDialog;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ContextMenuStrip changeInfoMenu;
		private System.Windows.Forms.ToolStripMenuItem changeToolStripMenuItem;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Timer dataRefreshTimer;
		private System.Windows.Forms.ContextMenuStrip toolBarContextMenu;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem imageToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem imageandTextToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem textToolStripMenuItem;
		private System.Windows.Forms.RichTextBox dataBox;
		private System.Windows.Forms.ToolStrip mainToolBar;
		private System.Windows.Forms.Panel collapseButtonHolder;
		private System.Windows.Forms.ToolStripMenuItem actionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem autoRefreshThreadListToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem autoRefreshStackTraceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem startMonitoringStackToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewStackToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ContextMenuStrip dataBoxContextMenu;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem1;
		private System.Windows.Forms.ToolStripButton toolThreadViewRefresh;
		private System.Windows.Forms.ToolStripButton toolStackRefresh;
		private System.Windows.Forms.ToolStripButton toolViewStack;
		private System.Windows.Forms.ToolStripButton toolSaveStack;
		private System.Windows.Forms.ToolStripButton toolMonitorStack;
		private System.Windows.Forms.ToolStripButton findButton;
		private ToolStripMenuItem takeStackSnapShotToolStripMenuItem;
		private ToolStripMenuItem viewThreadsToolStripMenuItem;
		private ContextMenuStrip threadViewContextMenu;
		private ToolStripMenuItem viewStackTraceToolStripMenuItem;
        private ToolStripMenuItem goToMSEHomepageToolStripMenuItem;
        private ToolStripMenuItem gotoMSEForumToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;

	}
}

