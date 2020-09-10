using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.Models
{
    public class Plants : Dictionary<string, string>
    {
        private static Plants _instance;
        public static Plants Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Plants();
                }
                return _instance;
            }
        }

        private Plants()
        {
            Add("0301", "Long An");
            Add("0302", "Long An");
            Add("0303", "Long An");
            Add("0304", "Long An");
            Add("0305", "Long An");
            Add("0306", "Long An");
            Add("0308", "Long An");
            Add("0501", "LiveStock");
        }
    }
}