namespace PrSQl
{
    partial class Identification
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Identification));
            this.labelUs = new System.Windows.Forms.Label();
            this.textBoxlog = new System.Windows.Forms.TextBox();
            this.labelPas = new System.Windows.Forms.Label();
            this.textBoxPas = new System.Windows.Forms.TextBox();
            this.buttonIn = new System.Windows.Forms.Button();
            this.buttonOut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelUs
            // 
            this.labelUs.AutoSize = true;
            this.labelUs.BackColor = System.Drawing.Color.DarkOrchid;
            this.labelUs.Font = new System.Drawing.Font("Lobster", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelUs.Location = new System.Drawing.Point(18, 40);
            this.labelUs.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelUs.Name = "labelUs";
            this.labelUs.Size = new System.Drawing.Size(136, 25);
            this.labelUs.TabIndex = 0;
            this.labelUs.Text = "Пользователь";
            // 
            // textBoxlog
            // 
            this.textBoxlog.AcceptsReturn = true;
            this.textBoxlog.Font = new System.Drawing.Font("Lobster", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxlog.Location = new System.Drawing.Point(207, 37);
            this.textBoxlog.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxlog.Name = "textBoxlog";
            this.textBoxlog.Size = new System.Drawing.Size(178, 32);
            this.textBoxlog.TabIndex = 1;
            this.textBoxlog.TextChanged += new System.EventHandler(this.textBoxlog_TextChanged);
            // 
            // labelPas
            // 
            this.labelPas.AutoSize = true;
            this.labelPas.BackColor = System.Drawing.Color.DarkOrchid;
            this.labelPas.Font = new System.Drawing.Font("Lobster", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelPas.Location = new System.Drawing.Point(22, 105);
            this.labelPas.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelPas.Name = "labelPas";
            this.labelPas.Size = new System.Drawing.Size(76, 25);
            this.labelPas.TabIndex = 2;
            this.labelPas.Text = "Пароль";
            // 
            // textBoxPas
            // 
            this.textBoxPas.AcceptsReturn = true;
            this.textBoxPas.Font = new System.Drawing.Font("Lobster", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxPas.Location = new System.Drawing.Point(207, 109);
            this.textBoxPas.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxPas.Name = "textBoxPas";
            this.textBoxPas.PasswordChar = '*';
            this.textBoxPas.Size = new System.Drawing.Size(176, 32);
            this.textBoxPas.TabIndex = 1;
            // 
            // buttonIn
            // 
            this.buttonIn.Font = new System.Drawing.Font("Lobster", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonIn.Location = new System.Drawing.Point(44, 266);
            this.buttonIn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonIn.Name = "buttonIn";
            this.buttonIn.Size = new System.Drawing.Size(150, 45);
            this.buttonIn.TabIndex = 4;
            this.buttonIn.Text = "Ок";
            this.buttonIn.UseVisualStyleBackColor = true;
            this.buttonIn.Click += new System.EventHandler(this.Button1_Click);
            // 
            // buttonOut
            // 
            this.buttonOut.Font = new System.Drawing.Font("Lobster", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonOut.Location = new System.Drawing.Point(231, 266);
            this.buttonOut.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonOut.Name = "buttonOut";
            this.buttonOut.Size = new System.Drawing.Size(129, 43);
            this.buttonOut.TabIndex = 5;
            this.buttonOut.Text = "Отмена";
            this.buttonOut.UseVisualStyleBackColor = true;
            this.buttonOut.Click += new System.EventHandler(this.Button2_Click);
            // 
            // Identification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::PrSQl.Properties.Resources.steam_backgrounds_2;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(426, 402);
            this.Controls.Add(this.buttonOut);
            this.Controls.Add(this.buttonIn);
            this.Controls.Add(this.textBoxPas);
            this.Controls.Add(this.labelPas);
            this.Controls.Add(this.textBoxlog);
            this.Controls.Add(this.labelUs);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Identification";
            this.Text = "Вход в приложение";
            this.Activated += new System.EventHandler(this.Form2_Activated);
            this.Load += new System.EventHandler(this.Identification_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        #endregion

        private System.Windows.Forms.Label labelUs;
        public System.Windows.Forms.TextBox textBoxlog;
        private System.Windows.Forms.Label labelPas;
        public System.Windows.Forms.TextBox textBoxPas;
        private System.Windows.Forms.Button buttonIn;
        private System.Windows.Forms.Button buttonOut;
    }
}