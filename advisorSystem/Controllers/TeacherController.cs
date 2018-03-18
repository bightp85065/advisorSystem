using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using advisorSystem.Models;
using advisorSystem.lib;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace advisorSystem.Controllers
{
    [Authorize(Roles = "teacher")]
    public class TeacherController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private String tid;
        
        public TeacherController()
        {
            if(User!=null)
                tid = User.Identity.GetUserId();
        }
        [Authorize(Roles = "teacher")]
        public ActionResult Index()
        {
            tid = User.Identity.GetUserId();
            System.Diagnostics.Debug.Print("user id " + tid);
            //get teacher student
            TeacherHelper teacherHelper = new TeacherHelper();
            JObject studnetlist = teacherHelper.GetStudent(tid);

            if ((bool)studnetlist["status"])
            {
                ViewBag.teacherStudent = studnetlist["data"].ToString(Formatting.None);
                return View();
            }
            else
            {
                String msg = studnetlist["msg"].ToString();
                return Content(msg);
            }  
        }
        [HttpGet]
        [Authorize(Roles = "teacher")]
        public String GetStudent()
        {
            tid = User.Identity.GetUserId();
            TeacherHelper teacherHelper = new TeacherHelper();
            JObject studnetlist = teacherHelper.GetStudent(tid);
            if ((bool)studnetlist["status"])
                return studnetlist["data"].ToString(Formatting.None);
            else
                return studnetlist["msg"].ToString();
        }
        [HttpGet]
        [Authorize(Roles = "teacher")]
        public String GetStudentApply()
        {
            tid = User.Identity.GetUserId();
            TeacherHelper teacherHelper = new TeacherHelper();
            JObject apply = teacherHelper.GetApply(tid);

            if ((bool)apply["status"])
                return apply["data"].ToString(Formatting.None);
            else
                return apply["msg"].ToString();
        }
        [Authorize(Roles = "teacher")]
        public String GetStudentChange()
        {
            tid = User.Identity.GetUserId();
            TeacherHelper teacherHelper = new TeacherHelper();
            JObject change = teacherHelper.GetChange(tid);

            if ((bool)change["status"])
                return change["data"].ToString(Formatting.None);
            else
                return change["msg"].ToString();
        }
        [Authorize(Roles = "teacher")]
        public String UpdateStudentApply(String tg_id, String s_id, int accept)
        {
            tid = User.Identity.GetUserId();
            TeacherHelper teacherHelper = new TeacherHelper();
            JObject update_apply = teacherHelper.UpdateApply(tid, tg_id, accept);

            //check if all teacher agree for application, 
            /* CheckAllApply get 0 means all student apply in accept */
            if (teacherHelper.CheckAllApply(tg_id) == 0)
            {
                //add new pair
                teacherHelper.AddApplyPair(tg_id, s_id);
                //update student apply history
                teacherHelper.UpdateStudentApplyHistory(tg_id, state:1); //state=1 means success
            }
            else
            {
                //全部都完成了，而且有老師拒絕
                //check if all teacher are check and there's rejection to apply
                //todo - update history student apply (state and ...?)
                //remove - 
            }

            return update_apply["status"].ToString(Formatting.None);
            //if ((bool)change["status"])
            //    return change["status"].ToString(Formatting.None);
            //else
            //    return change["msg"].ToString();
        }
        public String UpdateStudentChange(String sc_id, String org_tg_id, String s_id, String t_id, String thesis_state, String sc_allapproval, int accept)
        {
            tid = User.Identity.GetUserId();
            TeacherHelper teacherHelper = new TeacherHelper();

            //update teacher accpet according to allapprove
            JObject update_change = teacherHelper.UpdateChange(sc_id, s_id, t_id, thesis_state, sc_allapproval, accept);

            System.Diagnostics.Debug.Print((String)update_change["status"]);

            /*check if all original teacher agree for change advisor*/
             // if they all agree then add new pair 
            if (sc_allapproval.Equals("0") && teacherHelper.CheckOrgChange(sc_id) == 0)
            {
                // CheckAllApply get 0 means all student_apply is accepted
                // then update student apply allapprove to 1
                teacherHelper.UpdateStudentChangeApproval(sc_id);

            }else if(sc_allapproval.Equals("1") && teacherHelper.CheckNewChange(sc_id) == 0)
            {
                //remove original pair
                teacherHelper.removePair(s_id);
                //add new paired
                teacherHelper.AddChangePair(sc_id, s_id);
                //update student change history
                teacherHelper.UpdateStudentChangeHistory(org_tg_id, state:1); //state=1 means success
            }
            //todo - remove student_change - another query to check is all change checked
            
            return update_change["status"].ToString(Formatting.None);
            
        }
        public String GetApplyHistory()
        {
            tid = User.Identity.GetUserId();
            TeacherHelper teacherHelper = new TeacherHelper();
            JObject applyHistory = teacherHelper.GetApplyHistory(tid);
            if ((bool)applyHistory["status"])
                return applyHistory["data"].ToString(Formatting.None);
            else
                return applyHistory["msg"].ToString();
        }
        public String GetChangeHistory()
        {
            tid = User.Identity.GetUserId();
            TeacherHelper teacherHelper = new TeacherHelper();
            JObject changeHistory = teacherHelper.GetChangeHistory(tid);
            if ((bool)changeHistory["status"])
                return changeHistory["data"].ToString(Formatting.None);
            else
                return changeHistory["msg"].ToString();
        }
        public TeacherController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        
    }
}
 