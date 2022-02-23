using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIS_LOG_MANAGE
{
    [Serializable]
    public class SettingModel : XmlLoader<SettingModel>
    {
        /// <summary>
        /// 프로그램 로그 경로
        /// </summary>
        public string DaemonLogPath { get; set; }

        /// <summary>
        /// 데몬 로그 파일명
        /// </summary>
        public string DaemonLogFileName { get; set; }

        /// <summary>
        /// 데몬 로그 삭제
        /// </summary>
        public string DaemonLogFileNameDelete { get; set; }

        /// <summary>
        /// 로그의 복사 기간 설정 (ex : -1이면 현재기준 이전일 로그만 복사)
        /// </summary>
        public int MoveDays { get; set; }

        /// <summary>
        /// 로그의 보관 기간 설정 (ex : -3이면 3개월치 로그만 보관)
        /// </summary>
        public int DeleteMonths { get; set; }

        public List<SourceFolder> SourceFolderList { get; set; }
        public List<DestinationFolder> DestinationFolderList { get; set; }
    }

    public class SourceFolder
    {
        public string Src { get; set; }
    }

    public class DestinationFolder
    {
        public string Dest { get; set; }
    }
}
