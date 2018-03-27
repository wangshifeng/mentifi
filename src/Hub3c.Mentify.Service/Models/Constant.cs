using System.Diagnostics.CodeAnalysis;

namespace Hub3c.Mentify.Service.Models
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Constant
    {
        public const string USERTYPE_ADMIN = "Admin";
        public const string USERTYPE_ADVISER = "Adviser";
        public const string USERTYPE_CONTACT = "Contact";
        public const string USERTYPE_EMPLOYEE = "Employee";
        public const string USERTYPE_EXTERNAL = "External";
        public const string USERTYPE_SYSADMIN = "SysAdmin";
        public const string USERTYPE_USER = "User";

        // notification
        public const string NOTIFTYPE_ASSIGNMENT = "AS";
        public const string NOTIFTYPE_MENTIFIMESSAGEBOARD = "EM";
        public const string NOTIFTYPE_MENTIFIMESSAGEBOARDREQUESTSENT = "RS";
        public const string NOTIFTYPE_AUDIOCALL = "AC";
        public const string NOTIFTYPE_BUSINESSLINK = "BL";
        public const string NOTIFTYPE_BUSINESSJOIN = "BJ";
        public const string NOTIFTYPE_BUSINESSQR = "BQ";
        public const string NOTIFTYPE_EDUCONNECT = "EC";
        public const string NOTIFTYPE_EDUACCEPTCONNECT = "EA";
        public const string NOTIFTYPE_EDUREJECT = "ER";
        public const string NOTIFTYPE_CHANGEPROJECTACTIVITY = "CA";
        public const string NOTIFTYPE_CHAT = "CH";
        public const string NOTIFTYPE_CLIENTLINK = "CL";
        public const string NOTIFTYPE_DOCUMENTACCESS = "DA";
        public const string NOTIFTYPE_DOCUMENTWORKFLOWASSIGN = "DW";
        public const string NOTIFTYPE_DOCUMENTWORKFLOWUPDATE = "DU";
        public const string NOTIFTYPE_EVENTINVITATION = "EI";
        public const string NOTIFTYPE_FILETRANSFER = "FT";
        public const string NOTIFTYPE_HIRECONTRACTOR = "HC";
        public const string NOTIFTYPE_INVITEACTIVATE = "IA";
        public const string NOTIFTYPE_LEADS = "LD";
        public const string NOTIFTYPE_LINKACTIVATE = "LA";
        public const string NOTIFTYPE_MEETING = "MT";
        public const string NOTIFTYPE_MESSAGE = "MS";
        public const string NOTIFTYPE_MISSEDCALL = "MC";
        public const string NOTIFTYPE_NEXTACTIVITYSEQUENCE = "NA";
        public const string NOTIFTYPE_PRIVATEBULLETIN = "PV";
        public const string NOTIFTYPE_PRIVATEBULLETINREPLY = "PR";
        public const string NOTIFTYPE_PROJECTASSIGN = "PA";
        public const string NOTIFTYPE_QUICKTASK = "QT";
        public const string NOTIFTYPE_VIDEOCALL = "VC";
        public const string NOTIFTYPE_WORKFLOWSHARE = "WS";
        public const string NOTIFTYPE_J2JMESSAGEBOARD = "MB";
        public const string NOTIFTYPE_J2JAPPLYJOB = "AJ";
        public const string NOTIFTYPE_NEWLYJOINED = "NJ";
        public const string NOTIFTYPE_3RDPARTY = "3P";
        public const string NOTIFTYPE_ACTIVITYASSIGN = "AA";

        public const string NOTIFTYPE_PROJECTAPPROVED = "PA";
        public const string NOTIFTYPE_PROJECTDECLINED = "PD";
        public const string NOTIFTYPE_PROJECTACCEPTED = "PC";
        public const string NOTIFTYPE_PROJECTREJECTED = "PJ";
        public const string NOTIFTYPE_PROJECTCANCELED = "PX";
        public const string NOTIFTYPE_PROJECTPOSTED = "PP";
        public const string NOTIFTYPE_J2JPROJECTSTART = "PS";
        public const string REMOVE_CONNECTION_BY_MENTOR_OR_MENTEE_TO = "G6";
        public const string REMOVE_CONNECTION_BY_MENTOR_OR_MENTEE_FROM = "G7";
        public const string AcceptBusinessConnection = "AR";
        public const string RecommendToConnect = "RC";

        public const string LookupTypeCode_EduSubject = "EduSubject";
        public const string LookupTypeCode_EduAdditionalActivity = "EduAdditionalActivity";
        public const string LookupTypeCode_EduGrade = "EduGrade";
        public const string LookupTypeCode_EduInformalExperience = "EduInformalExperience";
        public const string LookupTypeCode_EduMatchedStatus = "EduMatchedStatus";
        public const string LookupTypeCode_EduModeOfStudy = "EduModeOfStudy";
        public const string LookupTypeCode_Salutation = "Salutation";
        public const string STATIC_EDURESUMEDOC = "Edu Resume";
        public const string STATIC_BULLETINATTACHMENT = "Bulletin Board";

        public struct GlobalNotification
        {
            public const string AcceptBusinessConnection = "AR";
            public const string RecommendToConnect = "RC";
        }

        public struct MentifiNotification
        {
            public const string NEW_USER_JOINED = "MJ";
            public const string GOAL_ADDED = "GA";
            public const string GOAL_EDITED = "GE";
            public const string GOAL_PROGRESS_ADDED = "GP";
            public const string GOAL_REMOVED = "GR";

            public const string TASK_CREATED = "T1";
            public const string TASK_ASSIGNED = "T2";
            public const string TASK_COMPLETED = "T3";
            public const string TASK_CONVERTED_TO_PROJECT = "T4";

            // Global Notification (Prefix G)
            public const string ADDED_CONNECTION_BY_UNIVERSITY = "G1";
            public const string REMOVE_CONNECTION_BY_UNIVERSITY = "G2";
            public const string ADDED_CONNECTION_BY_UNIVERSITY_REACHED_THE_LIMIT = "G3";
            public const string ACCEPT_CONNECTION_BY_UNIVERSITY = "G4";
            public const string ACCEPT_CONNECTION_BY_UNIVERSITY_REACHED_THE_LIMIT = "G5";
            public const string REMOVE_CONNECTION_BY_MENTOR_OR_MENTEE_TO = "G6";
            public const string REMOVE_CONNECTION_BY_MENTOR_OR_MENTEE_FROM = "G7";
        }

        public struct ProjectNotification
        {
            public const string CreateProject = "CP";
            public const string ChangeStatus = "CS";
            public const string ChangeDueDate = "DD";
            public const string AddTeamMember = "TM";
            public const string RemoveTeamMember = "RT";
            public const string ChangeRole = "CT";
            public const string BulletinPost = "NB";
            public const string NewActivity = "AC";
            public const string CompletedActivity = "CA";
            public const string NewNote = "NN";
            public const string ModifyNote = "MN";
            public const string NewAttachment = "PA";
            public const string RemoveAttachment = "RA";
            public const string NewAgenda = "NA";
            public const string ModifyAgenda = "MA";
            public const string DeleteAgenda = "DAG";
            public const string NewTimesheet = "NT";
            public const string UpdateTimesheet = "UT";
            public const string ChangeActivity = "CPA";
            public const string RenameProjectAttachment = "RPA";
            public const string ChangeActivityDueDate = "CAD";
            public const string ChangeAssignTo = "CAT";
            public const string DeleteActivity = "DA";
        }
    }
  
}
