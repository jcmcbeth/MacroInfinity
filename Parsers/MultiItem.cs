using System;
using System.Collections.Generic;
using System.Text;

namespace Infantry.Items
{
    /// <summary>
    /// The way an item should be picked up.
    /// </summary>
    public enum PickupMode
    {
        /// <summary>
        /// 
        /// </summary>
        Manual			= 0,

        /// <summary>
        /// 
        /// </summary>
        ManualAutoAll	= 1,

        /// <summary>
        ///
        /// </summary>
        ManualAutoNeed	= 2,

        /// <summary>
        /// 
        /// </summary>
        AutoAll		    = 3,

        /// <summary>
        /// 
        /// </summary>
        AutoNeed		= 4,

        /// <summary>
        /// 
        /// </summary>
        AutoHaveNone	= 5,
    }
    /*
     * 
******** Multi Item ********
Item Type		// Type of item 	
Version			// probably the item's version it was saved under
Item ID			// Unique Number Identifier
Name			// Name of the item
Category		// Category of the item, used in the buy list
Skill Logic		// Who can use the item, classes and such
Description		// Description of the item

Weight			// The weight of the item, *1000
Buy Price		// 0 is unpurchasable
Probability		// -1, never hide or allow in inventory, 0 is hide
Dropable		// 0 is cannot be droped, anything else is can be dropped
Key Preference		// Default key it is assigned to, 0 for no preference
Recommended		// Recommended quantity used to prioritize auto dumping
Max Allowed		// Max allowed in inventory, 0 is unlimited, negative numbers equal absolute limit, positive is usable limit
Pickup Mode		// How to handling pick up the item
Sell Price		// -1 no buy back, 0 will take it for free, item has to be purchasable to be sellable
Expire Timer		// How long an item will stay in your inventory before it expires, 0 is never, if dropped it disappears

Blob File		// The blob file the cfs file is in
Filename		// The filename of the cfs
Light Permutation	// Light and color intensity, 0-23 white, 24-47 red, 48-71 green, 72-95 blue, higher is full
Palette Offset		// Negative numbers mean special effects
Hue			// Hue Shift
Saturation		// Saturation shift
Value			// Value Shift
Animation Time		// Animation time in ticks, 0 to use cfs file

Cash			// Amount of money the item is worth, can be negative
Energy			// Amount of Energy the item is worth, can be negative
Health			// Amount of Health the item is worth, can be negative
Repair			// Amount of Repair the item is worth, if in a vehicle, can be negative
Experience		// Amount of experience the item is worth, can be negative

Recieved Items		// The ID of the item the player will recieve, 16 times
*****************************
    */
    public class MultiItem
    {        
        private int _cashWorth;
        private int _energyWorth;
        private int _healthWorth;
        private int _repairWorth;
        private int _experienceWorth;

        /// <summary>
        /// 
        /// </summary>
        public int CashWorth
        {
            get { return _cashWorth; }
            set { _cashWorth = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int EnergyWorth
        {
            get { return _energyWorth; }
            set { _energyWorth = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int HealthWorth
        {
            get { return _healthWorth; }
            set { _healthWorth = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int RepairWorth
        {
            get { return _repairWorth; }
            set { _repairWorth = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ExperienceWorth
        {
            get { return _experienceWorth; }
            set { _experienceWorth = value; }
        }

        /*
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

            this.CashWorth = Int32.Parse(data[25]);
            this.EnergyWorth = Int32.Parse(data[26]);
            this.HealthWorth = Int32.Parse(data[27]);
            this.RepairWorth = Int32.Parse(data[28]);
            this.ExperienceWorth = Int32.Parse(data[29]);
        }
         */
    }
}
