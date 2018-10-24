using System;
using System.Collections.Generic;
using System.Text;

namespace Infantry.Items
{
    /*
     *********************************************************************************
*                               Ammo Item                                      *
********************************************************************************
Index	Field			Description
0	Item Type		Type of item 	
1	Version			probably the item's version it was saved under
2	Item ID			Unique Number Identifier
3	Name			Name of the item
4	Category		Category of the item, used in the buy list
5	Skill Logic		Who can use the item, classes and such
6	Description		Description of the item

				General Information
7	Weight			The weight of the item, *1000
8	Buy Price		0 is unpurchasable
9	Probability		-1, never hide or allow in inventory, 0 is hide
10	Dropable		0 is cannot be droped, anything else is can be dropped
11	Key Preference		Default key it is assigned to, 0 for no preference
12	Recommended		Recommended quantity used to prioritize auto dumping
13	Max Allowed		Max allowed in inventory, 0 is unlimited, negative numbers equal absolute limit, positive is usable limit
14	Pickup Mode		How to handling pick up the item
15	Sell Price		-1 no buy back, 0 will take it for free, item has to be purchasable to be sellable
16	Expire Timer		How long an item will stay in your inventory before it expires, 0 is never, if dropped it disappears

				The Icon Image
17	Blob File		The blob file the cfs file is in
18	Filename		The filename of the cfs
19	Light Permutation	Light and color intensity, 0-23 white, 24-47 red, 48-71 green, 72-95 blue, higher is full
20	Palette Offset		Negative numbers mean special effects
21	Hue			Hue Shift
22	Saturation		Saturation shift
23	Value			Value Shift
24	Animation Time		Animation time in ticks, 0 to use cfs file
     */
    public class AmmoItem : Item
    {
        public AmmoItem()
        {            
        }

        /// <summary>
        /// Parses a comma seperated string into the values of the item.
        /// </summary>
        /// <remarks>Uses the functionality of the <see cref="Item.ParseLine"/></remarks>
        /// <param name="line">A line from an item file.</param>
        public override void ParseItem(string line)
        {
            base.ParseItem(line);
        }
    }
}
