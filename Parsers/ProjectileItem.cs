using System;
using System.Collections.Generic;
using System.Text;

namespace Infantry.Items
{
    public class ProjectileItem : Item
    {
        public ProjectileItem()
        {
        }

        /// <summary>
        /// Parses a comma seperated string into the values of the item.
        /// </summary>
        /// <remarks>Uses the functionality of the <see cref="Item.ParseLine"/></remarks>
        /// <param name="line">A line from an item file.</param>
        public override void ParseItem(string line)
        {
            string[] data;

            data = Item.ParseLine(line);

            base.ParseItem(line);            
        }
    }
}
