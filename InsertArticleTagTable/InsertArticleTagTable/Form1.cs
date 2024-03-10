using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InsertArticleTagTable
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        TravelEntities _context = new TravelEntities();
        private void insert_Click(object sender, EventArgs e)
        {
            var ct = classTextBox.Text;
            var na = NameTextBox.Text;

            ArticleTagList tag = _context.ArticleTagList.Create();

            tag.TagClass = ct;
            tag.TagName = na;

            _context.ArticleTagList.Add(tag);
            
            // 將新增的資料顯示在 dataGridView1 中
            dataGridView1.DataSource = _context.ArticleTagList.ToList();
            textBox1.Text = "插入成功!";
        }

        private void fill_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = _context.ArticleTagList.ToList();
        }

        private void save_Click(object sender, EventArgs e)
        {
            _context.SaveChanges();
            textBox1.Text = "保存成功!";
        }

        private void update_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = _context.ArticleTagList.ToList();
        }

        //參考 : https://blog.csdn.net/qq_23833037/article/details/52167239
        private void detel_Click(object sender, EventArgs e)
        {

            var id = dataGridView1.CurrentCell.Value;
            try
            {
                ArticleTagList art = _context.ArticleTagList.Find(id);
                _context.ArticleTagList.Remove(art);
                textBox1.Text = $"刪除{art.TagClass + art.TagName}成功!";
               
            }
            catch
            {
                MessageBox.Show("指定錯誤!");
            }

        }
    }
}

/*// 創建 ArticleTagList 物件並新增至資料庫
            List<ArticleTagList> tagList = new List<ArticleTagList>
            {
                new ArticleTagList {TagClass = "life", TagName = "一日遊"},
                new ArticleTagList {TagClass = "life", TagName = "半日遊"}
                //new ArticleTagList { TagClass = "city", TagName= "嘉義市" },
                //new ArticleTagList { TagClass = "city", TagName= "桃園市" },
                //new ArticleTagList { TagClass = "city", TagName= "臺北市" },
                //new ArticleTagList { TagClass = "city", TagName= "新北市" },
                //new ArticleTagList { TagClass = "city", TagName= "嘉義縣" },
                //new ArticleTagList { TagClass = "city", TagName= "澎湖縣" },
                //new ArticleTagList { TagClass = "city", TagName= "其它" },
                //new ArticleTagList { TagClass = "city", TagName= "金門縣" },
                //new ArticleTagList { TagClass = "city", TagName= "新竹縣" },
                //new ArticleTagList { TagClass = "city", TagName= "新竹市" },
                //new ArticleTagList { TagClass = "city", TagName= "花蓮縣" },
                //new ArticleTagList { TagClass = "city", TagName= "臺南市" },
                //new ArticleTagList { TagClass = "city", TagName= "高雄市" },
                //new ArticleTagList { TagClass = "city", TagName= "基隆市" },
                //new ArticleTagList { TagClass = "city", TagName= "南投縣" },
                //new ArticleTagList { TagClass = "city", TagName= "宜蘭縣" },
                //new ArticleTagList { TagClass = "city", TagName= "苗栗縣" },
                //new ArticleTagList { TagClass = "city", TagName= "雲林縣" },
                //new ArticleTagList { TagClass = "city", TagName= "連江縣" },
                //new ArticleTagList { TagClass = "city", TagName= "臺中市" },
                //new ArticleTagList { TagClass = "city", TagName= "屏東縣" },
                //new ArticleTagList { TagClass = "city", TagName= "彰化縣" },
                //new ArticleTagList { TagClass = "city", TagName= "臺東縣" }
            };


            foreach (var item in tagList)
            {
                _context.ArticleTagList.Add(item);
            }*/