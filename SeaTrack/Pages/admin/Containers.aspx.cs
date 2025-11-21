using SeaTrack.DAL;
using SeaTrack.Utilities;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace SeaTrack.Pages.admin
{
    public partial class Containers : System.Web.UI.Page
    {
        // The instance variable has been removed because the repository methods are static.
        // private ContainerRepository containerRepo = new ContainerRepository();

        private int? EditContainerId
        {
            get { return ViewState["EditContainerId"] as int?; }
            set { ViewState["EditContainerId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSizes();      
                LoadContainers();
            }
        }

        private void LoadSizes()
        {
            // Call the method statically using the class name.
            DataTable dt = ContainerRepository.GetAllContainerSizes();
            ddlSize.DataSource = dt;
            ddlSize.DataBind();
        }

        private void LoadContainers()
        {
            // Call the method statically using the class name.
            DataTable dt = ContainerRepository.GetAllContainersWithDetails();
            gvContainers.DataSource = dt;
            gvContainers.DataBind();
        }

        protected void btnAddContainer_Click(object sender, EventArgs e)
        {
            pnlAddContainer.Visible = true;
            ClearForm();
            EditContainerId = null;
        }

        protected void btnSaveContainer_Click(object sender, EventArgs e)
        {
            try
            {
                if (EditContainerId.HasValue)
                {
                    // تحديث - Call the method statically.
                    ContainerRepository.UpdateContainer(
                        EditContainerId.Value,
                        txtContainerCode.Text.Trim(),
                        int.Parse(ddlContainerType.SelectedValue),
                        int.Parse(ddlSize.SelectedValue),
                        decimal.Parse(txtBaseWeight.Text),
                        decimal.Parse(txtMaxWeight.Text),
                        chkIsAvailable.Checked
                    );
                }
                else
                {
                    // إضافة جديد - Call the method statically.
                    ContainerRepository.AddContainer(
                        txtContainerCode.Text.Trim(),
                        int.Parse(ddlContainerType.SelectedValue),
                        int.Parse(ddlSize.SelectedValue),
                        decimal.Parse(txtBaseWeight.Text),
                        decimal.Parse(txtMaxWeight.Text),
                        chkIsAvailable.Checked
                    );
                }

                pnlAddContainer.Visible = false;
                LoadContainers();
                ClearForm();
            }
            catch (Exception ex)
            {
                // CORRECTED: Swapped the arguments for LogError.
                LogHelper.LogError("Containers.aspx.cs - btnSaveContainer_Click", ex);
            }
        }

        protected void btnCancelContainer_Click(object sender, EventArgs e)
        {
            pnlAddContainer.Visible = false;
            ClearForm();
            EditContainerId = null;
        }

        protected void gvContainers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int containerId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditContainer")
            {
                EditContainerId = containerId;
                LoadContainerForEdit(containerId);
                pnlAddContainer.Visible = true;
            }
            else if (e.CommandName == "DeleteContainer")
            {
                // Call the method statically.
                ContainerRepository.DeleteContainer(containerId);
                LoadContainers();
            }
        }

        private void LoadContainerForEdit(int containerId)
        {
            // Call the method statically.
            DataRow row = ContainerRepository.GetContainerById(containerId);
            if (row != null)
            {
                txtContainerCode.Text = row["container_code"].ToString();
                ddlContainerType.SelectedValue = row["type"].ToString();
                ddlSize.SelectedValue = row["size_id"].ToString();
                txtBaseWeight.Text = row["base_weight_kg"].ToString();
                txtMaxWeight.Text = row["max_weight_kg"].ToString();
                chkIsAvailable.Checked = Convert.ToBoolean(row["is_available"]);
            }
        }

        private void ClearForm()
        {
            txtContainerCode.Text = "";
            ddlContainerType.SelectedIndex = 0;

            // التعديل: التحقق من وجود عناصر قبل تحديد الفهرس
            if (ddlSize.Items.Count > 0)
            {
                ddlSize.SelectedIndex = 0;
            }
            else
            {
                ddlSize.ClearSelection(); // مجرد إلغاء التحديد إذا كانت فارغة
            }

            txtBaseWeight.Text = "";
            txtMaxWeight.Text = "";
            chkIsAvailable.Checked = true;
        }
    }
}