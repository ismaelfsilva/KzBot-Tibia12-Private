namespace KzBot.UI
{
    partial class Targeting
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
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.knightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paladinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exetaResToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exetaAmpResToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader5,
            this.columnHeader7,
            this.columnHeader6,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader8,
            this.columnHeader9});
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = true;
            this.listView1.Location = new System.Drawing.Point(12, 12);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(318, 120);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 0;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Type";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 40;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Key";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader5.Width = 50;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Center";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader7.Width = 50;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Range";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader6.Width = 50;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Count";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 50;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Mana";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader4.Width = 50;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Level";
            this.columnHeader8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader8.Width = 0;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Delay";
            this.columnHeader9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader9.Width = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator1,
            this.knightToolStripMenuItem,
            this.paladinToolStripMenuItem,
            this.mageToolStripMenuItem,
            this.toolStripSeparator2,
            this.exetaResToolStripMenuItem,
            this.exetaAmpResToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 170);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // knightToolStripMenuItem
            // 
            this.knightToolStripMenuItem.Name = "knightToolStripMenuItem";
            this.knightToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.knightToolStripMenuItem.Text = "Knight";
            this.knightToolStripMenuItem.Click += new System.EventHandler(this.knightToolStripMenuItem_Click);
            // 
            // paladinToolStripMenuItem
            // 
            this.paladinToolStripMenuItem.Name = "paladinToolStripMenuItem";
            this.paladinToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.paladinToolStripMenuItem.Text = "Paladin";
            this.paladinToolStripMenuItem.Click += new System.EventHandler(this.paladinToolStripMenuItem_Click);
            // 
            // mageToolStripMenuItem
            // 
            this.mageToolStripMenuItem.Enabled = false;
            this.mageToolStripMenuItem.Name = "mageToolStripMenuItem";
            this.mageToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.mageToolStripMenuItem.Text = "Mage";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
            // 
            // exetaResToolStripMenuItem
            // 
            this.exetaResToolStripMenuItem.Name = "exetaResToolStripMenuItem";
            this.exetaResToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exetaResToolStripMenuItem.Text = "Exeta Res";
            this.exetaResToolStripMenuItem.Click += new System.EventHandler(this.exetaResToolStripMenuItem_Click);
            // 
            // exetaAmpResToolStripMenuItem
            // 
            this.exetaAmpResToolStripMenuItem.Name = "exetaAmpResToolStripMenuItem";
            this.exetaAmpResToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exetaAmpResToolStripMenuItem.Text = "Exeta Amp Res";
            this.exetaAmpResToolStripMenuItem.Click += new System.EventHandler(this.exetaAmpResToolStripMenuItem_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(260, 166);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(70, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "New";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(185, 139);
            this.textBox2.Name = "textBox2";
            this.textBox2.PlaceholderText = "Min Creatures";
            this.textBox2.Size = new System.Drawing.Size(69, 23);
            this.textBox2.TabIndex = 5;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(185, 166);
            this.textBox1.Name = "textBox1";
            this.textBox1.PlaceholderText = "Mana";
            this.textBox1.Size = new System.Drawing.Size(69, 23);
            this.textBox1.TabIndex = 6;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBox1.Enabled = false;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(12, 166);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(84, 23);
            this.comboBox1.TabIndex = 7;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(260, 138);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(70, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "None";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.button2_KeyDown);
            // 
            // textBox3
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox3.Enabled = false;
            this.textBox3.Location = new System.Drawing.Point(102, 139);
            this.textBox3.Name = "textBox3";
            this.textBox3.PlaceholderText = "Skill Range";
            this.textBox3.Size = new System.Drawing.Size(77, 23);
            this.textBox3.TabIndex = 9;
            this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // comboBox2
            // 
            this.comboBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBox2.Enabled = false;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "Target",
            "Player"});
            this.comboBox2.Location = new System.Drawing.Point(12, 139);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(84, 23);
            this.comboBox2.TabIndex = 10;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // textBox4
            // 
            this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox4.Enabled = false;
            this.textBox4.Location = new System.Drawing.Point(102, 166);
            this.textBox4.Name = "textBox4";
            this.textBox4.PlaceholderText = "Creatures";
            this.textBox4.Size = new System.Drawing.Size(77, 23);
            this.textBox4.TabIndex = 11;
            // 
            // Targeting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(342, 201);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Targeting";
            this.Text = "Targeting";
            this.Load += new System.EventHandler(this.Targeting_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Button button1;
        private TextBox textBox2;
        private TextBox textBox1;
        private ComboBox comboBox1;
        private Button button2;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        public ListView listView1;
        private TextBox textBox3;
        private ComboBox comboBox2;
        private TextBox textBox4;
        private ColumnHeader columnHeader7;
        private ColumnHeader columnHeader6;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem clearToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ColumnHeader columnHeader8;
        private ColumnHeader columnHeader9;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem knightToolStripMenuItem;
        private ToolStripMenuItem paladinToolStripMenuItem;
        private ToolStripMenuItem mageToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exetaResToolStripMenuItem;
        private ToolStripMenuItem exetaAmpResToolStripMenuItem;
    }
}