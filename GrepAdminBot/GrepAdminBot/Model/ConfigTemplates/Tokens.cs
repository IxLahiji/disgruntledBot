using System;

namespace GrepAdminBot.Model.ConfigTemplates
{
    public class Tokens
    {
        public string discord { get; set; }
        public string giphy { get; set; }

        public Tokens(string discord=null, string giphy=null)
        {
            this.discord = discord;
            this.giphy = giphy;
        }
    }
}
