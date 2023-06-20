namespace SharpMap_Test
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.mapBox1 = new SharpMap.Forms.MapBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.richTextBoxLayerList = new System.Windows.Forms.RichTextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.richTextBoxPointLayerList = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonClickModePan = new System.Windows.Forms.RadioButton();
            this.radioButtonClickModeSelect = new System.Windows.Forms.RadioButton();
            this.radioButtonClickModeDraw = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mapBox1
            // 
            this.mapBox1.ActiveTool = SharpMap.Forms.MapBox.Tools.None;
            this.mapBox1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.mapBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.mapBox1.CustomTool = null;
            this.mapBox1.FineZoomFactor = 10D;
            this.mapBox1.Location = new System.Drawing.Point(12, 41);
            this.mapBox1.MapQueryMode = SharpMap.Forms.MapBox.MapQueryType.LayerByIndex;
            this.mapBox1.Name = "mapBox1";
            this.mapBox1.QueryGrowFactor = 5F;
            this.mapBox1.QueryLayerIndex = 0;
            this.mapBox1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.mapBox1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.mapBox1.ShowProgressUpdate = false;
            this.mapBox1.Size = new System.Drawing.Size(776, 396);
            this.mapBox1.TabIndex = 0;
            this.mapBox1.Text = "mapBox1";
            this.mapBox1.WheelZoomMagnitude = -2D;
            this.mapBox1.MouseMove += new SharpMap.Forms.MapBox.MouseEventHandler(this.mapBox1_MouseMove);
            this.mapBox1.MouseDown += new SharpMap.Forms.MapBox.MouseEventHandler(this.mapBox1_MouseDown);
            this.mapBox1.MouseUp += new SharpMap.Forms.MapBox.MouseEventHandler(this.mapBox1_MouseUp);
            this.mapBox1.MapCenterChanged += new SharpMap.Forms.MapBox.MapCenterChangedHandler(this.mapBox1_MapCenterChanged);
            this.mapBox1.Click += new System.EventHandler(this.mapBox1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 491);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 24);
            this.label2.TabIndex = 2;
            this.label2.Text = "ピクセル座標\r\n→地理座標";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(308, 452);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(152, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "レイヤ全体を表示する";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(308, 481);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(152, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(308, 513);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(152, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "ベース以外のレイヤ初期化";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 457);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 24);
            this.label1.TabIndex = 7;
            this.label1.Text = "地理座標\r\n→ピクセル座標";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(794, 41);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(199, 23);
            this.button4.TabIndex = 8;
            this.button4.Text = "mapBox1のレイヤ一覧　更新";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // richTextBoxLayerList
            // 
            this.richTextBoxLayerList.Location = new System.Drawing.Point(795, 74);
            this.richTextBoxLayerList.Name = "richTextBoxLayerList";
            this.richTextBoxLayerList.Size = new System.Drawing.Size(334, 96);
            this.richTextBoxLayerList.TabIndex = 9;
            this.richTextBoxLayerList.Text = "";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(794, 176);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(199, 45);
            this.button5.TabIndex = 10;
            this.button5.Text = "pointLayerの一覧　更新\r\nlineStringLayerの一覧　更新";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // richTextBoxPointLayerList
            // 
            this.richTextBoxPointLayerList.Location = new System.Drawing.Point(795, 227);
            this.richTextBoxPointLayerList.Name = "richTextBoxPointLayerList";
            this.richTextBoxPointLayerList.Size = new System.Drawing.Size(333, 210);
            this.richTextBoxPointLayerList.TabIndex = 11;
            this.richTextBoxPointLayerList.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 524);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 12);
            this.label3.TabIndex = 14;
            this.label3.Text = "ヒット判定";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonClickModePan);
            this.groupBox1.Controls.Add(this.radioButtonClickModeSelect);
            this.groupBox1.Controls.Add(this.radioButtonClickModeDraw);
            this.groupBox1.Location = new System.Drawing.Point(483, 457);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(129, 126);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "クリックモード";
            // 
            // radioButtonClickModePan
            // 
            this.radioButtonClickModePan.AutoSize = true;
            this.radioButtonClickModePan.Location = new System.Drawing.Point(19, 81);
            this.radioButtonClickModePan.Name = "radioButtonClickModePan";
            this.radioButtonClickModePan.Size = new System.Drawing.Size(42, 16);
            this.radioButtonClickModePan.TabIndex = 2;
            this.radioButtonClickModePan.TabStop = true;
            this.radioButtonClickModePan.Text = "パン";
            this.radioButtonClickModePan.UseVisualStyleBackColor = true;
            this.radioButtonClickModePan.CheckedChanged += new System.EventHandler(this.radioButtonClickModePan_CheckedChanged);
            // 
            // radioButtonClickModeSelect
            // 
            this.radioButtonClickModeSelect.AutoSize = true;
            this.radioButtonClickModeSelect.Location = new System.Drawing.Point(19, 52);
            this.radioButtonClickModeSelect.Name = "radioButtonClickModeSelect";
            this.radioButtonClickModeSelect.Size = new System.Drawing.Size(87, 16);
            this.radioButtonClickModeSelect.TabIndex = 1;
            this.radioButtonClickModeSelect.Text = "点を選択する";
            this.radioButtonClickModeSelect.UseVisualStyleBackColor = true;
            // 
            // radioButtonClickModeDraw
            // 
            this.radioButtonClickModeDraw.AutoSize = true;
            this.radioButtonClickModeDraw.Location = new System.Drawing.Point(19, 24);
            this.radioButtonClickModeDraw.Name = "radioButtonClickModeDraw";
            this.radioButtonClickModeDraw.Size = new System.Drawing.Size(62, 16);
            this.radioButtonClickModeDraw.TabIndex = 0;
            this.radioButtonClickModeDraw.Text = "点を描く";
            this.radioButtonClickModeDraw.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(481, 596);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 17;
            this.label4.Text = "選択判定";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(794, 452);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(334, 183);
            this.richTextBox1.TabIndex = 18;
            this.richTextBox1.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1140, 654);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.richTextBoxPointLayerList);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.richTextBoxLayerList);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mapBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.RichTextBox richTextBoxLayerList;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.RichTextBox richTextBoxPointLayerList;
        public SharpMap.Forms.MapBox mapBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonClickModeSelect;
        private System.Windows.Forms.RadioButton radioButtonClickModeDraw;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton radioButtonClickModePan;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}

