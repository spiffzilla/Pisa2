using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PisaNamespace
{
    public class ArticleFactorCollection
    {
        private List<ArticleFactor> ArticleFactorList;
        private string values;

        private class ArticleFactor
        {
            public string Article { get; set; }
            public double Factor { get; set; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ArticleFactorCollection()
        {
            ArticleFactorList = new List<ArticleFactor>();
            ReadFactorValues();
        }

        public string GenerateArticleFactorConfigString()
        {
            var articleFactorString = string.Empty;
            foreach (ArticleFactor articleFactor in ArticleFactorList)
            {
                articleFactorString += string.Format("{0}:{1},", articleFactor.Article, articleFactor.Factor);
            }
            return articleFactorString.TrimEnd(',');
        }

        
        //////////////////////////
        public string[] ReadDataGridWithArticleFactors(DataGridView dataGridView,int row)
        {

            string[] valuePair = new string[2] { string.Empty, string.Empty };

            if (dataGridView == null || row >= dataGridView.Rows.Count || dataGridView.Rows[row] == null || dataGridView.Rows[row].Cells == null)
            {
                return valuePair;
            }
            if (dataGridView.Rows[row].Cells[0].Value != null)
            {
                valuePair[0] = dataGridView.Rows[row].Cells[0].Value.ToString();
            }
            if (dataGridView.Rows[row].Cells[1].Value != null)
            {
                valuePair[1] = dataGridView.Rows[row].Cells[1].Value.ToString();
            }
            return valuePair;
        }
        
        /////////////////////////////

        public bool FillDataGridWithArticleFactors(DataGridView dataGridView)
        {
            var result = false;
            try
            {
                // Populate datagridview
                dataGridView.Rows.Clear();

                for (int i = 0; i < ArticleFactorList.Count(); i++)
                {
                    dataGridView.Rows.Add();
                    dataGridView.Rows[i].Cells[0].Value = ArticleFactorList[i].Article;
                    dataGridView.Rows[i].Cells[1].Value = ArticleFactorList[i].Factor;
                }


                result = true;
            }
            catch (Exception)
            {
                // todo: add error handling
            }
            return result;
        }

        private bool ReadFactorValues()
        {
            // read value string and split up all values into array artikelFaktor
            values = ReadConfig();

            string[] pairs = values.Split(',');
            ArticleFactorList.Clear();
            for (int j = 0; j < pairs.Count(); j++)
            {
                var dataArr = pairs[j].Split(':');
                ArticleFactorList.Add(new ArticleFactor
                {
                    Article = dataArr[0],
                    Factor = Convert.ToDouble(dataArr[1])
                });
            }
            return true;
        }

        private string ReadConfig()
        {
            string ArtFac = Properties.Settings.Default.ArticleFactors.ToString();
            if (ArtFac == "") { ArtFac = "Konfiguration tom:000"; }
            return ArtFac;
        }
    }
}
