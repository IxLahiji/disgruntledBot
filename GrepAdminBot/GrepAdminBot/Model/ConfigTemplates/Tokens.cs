using System;

namespace GrepAdminBot.Model.ConfigTemplates
{
    public class Tokens
    {
        public string discord { get; set; }

        public Tokens(string discord=null)
        {
            this.discord = discord;
        }
    }
}
