namespace GadgetCentralSystem
{
    partial class Form1
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
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Updatebtn = new System.Windows.Forms.Button();
            this.Addbtn = new System.Windows.Forms.Button();
            this.Deletebtn = new System.Windows.Forms.Button();
            this.ImageUrltxt = new System.Windows.Forms.TextBox();
            this.ImageUrlLbl = new System.Windows.Forms.Label();
            this.Categorytxt = new System.Windows.Forms.TextBox();
            this.CategoryLbl = new System.Windows.Forms.Label();
            this.Descriptiontxt = new System.Windows.Forms.TextBox();
            this.DescriptionLbl = new System.Windows.Forms.Label();
            this.Stocktxt = new System.Windows.Forms.TextBox();
            this.StockLbl = new System.Windows.Forms.Label();
            this.Pricetxt = new System.Windows.Forms.TextBox();
            this.PriceLbl = new System.Windows.Forms.Label();
            this.Nametxt = new System.Windows.Forms.TextBox();
            this.NameLbl = new System.Windows.Forms.Label();
            this.ProductGlobalIdtxt = new System.Windows.Forms.TextBox();
            this.ProductGlobalLbl = new System.Windows.Forms.Label();
            this.ProductIdtxt = new System.Windows.Forms.TextBox();
            this.ProductIdLbl = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvItems
            // 
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1});
            this.dgvItems.Location = new System.Drawing.Point(386, 53);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.Size = new System.Drawing.Size(426, 176);
            this.dgvItems.TabIndex = 61;
            this.dgvItems.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItems_CellContentClick);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Edit";
            this.Column1.Name = "Column1";
            // 
            // Updatebtn
            // 
            this.Updatebtn.Location = new System.Drawing.Point(296, 411);
            this.Updatebtn.Name = "Updatebtn";
            this.Updatebtn.Size = new System.Drawing.Size(75, 23);
            this.Updatebtn.TabIndex = 60;
            this.Updatebtn.Text = "Update";
            this.Updatebtn.UseVisualStyleBackColor = true;
            this.Updatebtn.Click += new System.EventHandler(this.Updatebtn_Click);
            // 
            // Addbtn
            // 
            this.Addbtn.Location = new System.Drawing.Point(177, 411);
            this.Addbtn.Name = "Addbtn";
            this.Addbtn.Size = new System.Drawing.Size(75, 23);
            this.Addbtn.TabIndex = 59;
            this.Addbtn.Text = "Add";
            this.Addbtn.UseVisualStyleBackColor = true;
            this.Addbtn.Click += new System.EventHandler(this.Addbtn_Click);
            // 
            // Deletebtn
            // 
            this.Deletebtn.Location = new System.Drawing.Point(57, 411);
            this.Deletebtn.Name = "Deletebtn";
            this.Deletebtn.Size = new System.Drawing.Size(75, 23);
            this.Deletebtn.TabIndex = 58;
            this.Deletebtn.Text = "Delete";
            this.Deletebtn.UseVisualStyleBackColor = true;
            this.Deletebtn.Click += new System.EventHandler(this.Deletebtn_Click);
            // 
            // ImageUrltxt
            // 
            this.ImageUrltxt.Location = new System.Drawing.Point(172, 362);
            this.ImageUrltxt.Name = "ImageUrltxt";
            this.ImageUrltxt.Size = new System.Drawing.Size(100, 20);
            this.ImageUrltxt.TabIndex = 57;
            // 
            // ImageUrlLbl
            // 
            this.ImageUrlLbl.AutoSize = true;
            this.ImageUrlLbl.Location = new System.Drawing.Point(74, 365);
            this.ImageUrlLbl.Name = "ImageUrlLbl";
            this.ImageUrlLbl.Size = new System.Drawing.Size(49, 13);
            this.ImageUrlLbl.TabIndex = 56;
            this.ImageUrlLbl.Text = "ImageUrl";
            // 
            // Categorytxt
            // 
            this.Categorytxt.Location = new System.Drawing.Point(172, 316);
            this.Categorytxt.Name = "Categorytxt";
            this.Categorytxt.Size = new System.Drawing.Size(100, 20);
            this.Categorytxt.TabIndex = 55;
            // 
            // CategoryLbl
            // 
            this.CategoryLbl.AutoSize = true;
            this.CategoryLbl.Location = new System.Drawing.Point(74, 319);
            this.CategoryLbl.Name = "CategoryLbl";
            this.CategoryLbl.Size = new System.Drawing.Size(49, 13);
            this.CategoryLbl.TabIndex = 54;
            this.CategoryLbl.Text = "Category";
            // 
            // Descriptiontxt
            // 
            this.Descriptiontxt.Location = new System.Drawing.Point(172, 268);
            this.Descriptiontxt.Name = "Descriptiontxt";
            this.Descriptiontxt.Size = new System.Drawing.Size(100, 20);
            this.Descriptiontxt.TabIndex = 53;
            // 
            // DescriptionLbl
            // 
            this.DescriptionLbl.AutoSize = true;
            this.DescriptionLbl.Location = new System.Drawing.Point(74, 271);
            this.DescriptionLbl.Name = "DescriptionLbl";
            this.DescriptionLbl.Size = new System.Drawing.Size(60, 13);
            this.DescriptionLbl.TabIndex = 52;
            this.DescriptionLbl.Text = "Description";
            // 
            // Stocktxt
            // 
            this.Stocktxt.Location = new System.Drawing.Point(172, 223);
            this.Stocktxt.Name = "Stocktxt";
            this.Stocktxt.Size = new System.Drawing.Size(100, 20);
            this.Stocktxt.TabIndex = 51;
            // 
            // StockLbl
            // 
            this.StockLbl.AutoSize = true;
            this.StockLbl.Location = new System.Drawing.Point(74, 226);
            this.StockLbl.Name = "StockLbl";
            this.StockLbl.Size = new System.Drawing.Size(35, 13);
            this.StockLbl.TabIndex = 50;
            this.StockLbl.Text = "Stock";
            // 
            // Pricetxt
            // 
            this.Pricetxt.Location = new System.Drawing.Point(172, 181);
            this.Pricetxt.Name = "Pricetxt";
            this.Pricetxt.Size = new System.Drawing.Size(100, 20);
            this.Pricetxt.TabIndex = 49;
            // 
            // PriceLbl
            // 
            this.PriceLbl.AutoSize = true;
            this.PriceLbl.Location = new System.Drawing.Point(74, 184);
            this.PriceLbl.Name = "PriceLbl";
            this.PriceLbl.Size = new System.Drawing.Size(31, 13);
            this.PriceLbl.TabIndex = 48;
            this.PriceLbl.Text = "Price";
            // 
            // Nametxt
            // 
            this.Nametxt.Location = new System.Drawing.Point(172, 137);
            this.Nametxt.Name = "Nametxt";
            this.Nametxt.Size = new System.Drawing.Size(100, 20);
            this.Nametxt.TabIndex = 47;
            // 
            // NameLbl
            // 
            this.NameLbl.AutoSize = true;
            this.NameLbl.Location = new System.Drawing.Point(74, 140);
            this.NameLbl.Name = "NameLbl";
            this.NameLbl.Size = new System.Drawing.Size(35, 13);
            this.NameLbl.TabIndex = 46;
            this.NameLbl.Text = "Name";
            // 
            // ProductGlobalIdtxt
            // 
            this.ProductGlobalIdtxt.Location = new System.Drawing.Point(172, 94);
            this.ProductGlobalIdtxt.Name = "ProductGlobalIdtxt";
            this.ProductGlobalIdtxt.Size = new System.Drawing.Size(100, 20);
            this.ProductGlobalIdtxt.TabIndex = 45;
            // 
            // ProductGlobalLbl
            // 
            this.ProductGlobalLbl.AutoSize = true;
            this.ProductGlobalLbl.Location = new System.Drawing.Point(74, 97);
            this.ProductGlobalLbl.Name = "ProductGlobalLbl";
            this.ProductGlobalLbl.Size = new System.Drawing.Size(91, 13);
            this.ProductGlobalLbl.TabIndex = 44;
            this.ProductGlobalLbl.Text = "Product Global ID";
            // 
            // ProductIdtxt
            // 
            this.ProductIdtxt.Location = new System.Drawing.Point(172, 50);
            this.ProductIdtxt.Name = "ProductIdtxt";
            this.ProductIdtxt.Size = new System.Drawing.Size(100, 20);
            this.ProductIdtxt.TabIndex = 43;
            // 
            // ProductIdLbl
            // 
            this.ProductIdLbl.AutoSize = true;
            this.ProductIdLbl.Location = new System.Drawing.Point(74, 53);
            this.ProductIdLbl.Name = "ProductIdLbl";
            this.ProductIdLbl.Size = new System.Drawing.Size(58, 13);
            this.ProductIdLbl.TabIndex = 42;
            this.ProductIdLbl.Text = "Product ID";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(868, 485);
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.Updatebtn);
            this.Controls.Add(this.Addbtn);
            this.Controls.Add(this.Deletebtn);
            this.Controls.Add(this.ImageUrltxt);
            this.Controls.Add(this.ImageUrlLbl);
            this.Controls.Add(this.Categorytxt);
            this.Controls.Add(this.CategoryLbl);
            this.Controls.Add(this.Descriptiontxt);
            this.Controls.Add(this.DescriptionLbl);
            this.Controls.Add(this.Stocktxt);
            this.Controls.Add(this.StockLbl);
            this.Controls.Add(this.Pricetxt);
            this.Controls.Add(this.PriceLbl);
            this.Controls.Add(this.Nametxt);
            this.Controls.Add(this.NameLbl);
            this.Controls.Add(this.ProductGlobalIdtxt);
            this.Controls.Add(this.ProductGlobalLbl);
            this.Controls.Add(this.ProductIdtxt);
            this.Controls.Add(this.ProductIdLbl);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.DataGridViewButtonColumn Column1;
        private System.Windows.Forms.Button Updatebtn;
        private System.Windows.Forms.Button Addbtn;
        private System.Windows.Forms.Button Deletebtn;
        private System.Windows.Forms.TextBox ImageUrltxt;
        private System.Windows.Forms.Label ImageUrlLbl;
        private System.Windows.Forms.TextBox Categorytxt;
        private System.Windows.Forms.Label CategoryLbl;
        private System.Windows.Forms.TextBox Descriptiontxt;
        private System.Windows.Forms.Label DescriptionLbl;
        private System.Windows.Forms.TextBox Stocktxt;
        private System.Windows.Forms.Label StockLbl;
        private System.Windows.Forms.TextBox Pricetxt;
        private System.Windows.Forms.Label PriceLbl;
        private System.Windows.Forms.TextBox Nametxt;
        private System.Windows.Forms.Label NameLbl;
        private System.Windows.Forms.TextBox ProductGlobalIdtxt;
        private System.Windows.Forms.Label ProductGlobalLbl;
        private System.Windows.Forms.TextBox ProductIdtxt;
        private System.Windows.Forms.Label ProductIdLbl;
    }
}

