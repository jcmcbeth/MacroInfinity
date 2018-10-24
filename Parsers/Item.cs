using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Infantry.Items
{
    /// <summary>
    /// The types of items.
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// 
        /// </summary>
        Multi		= 1,

        /// <summary>
        /// 
        /// </summary>
        Ammo		= 4,

        /// <summary>
        /// 
        /// </summary>
        Projectile	= 6,

        /// <summary>
        /// 
        /// </summary>
        VehicleMaker= 7,

        /// <summary>
        /// 
        /// </summary>
        MultiUse	= 8,

        /// <summary>
        /// 
        /// </summary>
        Repair		= 11,

        /// <summary>
        /// 
        /// </summary>
        Control		= 12,

        /// <summary>
        /// 
        /// </summary>
        Utility		= 13,

        /// <summary>
        /// 
        /// </summary>
        Maker		= 14,

        /// <summary>
        /// 
        /// </summary>
        Upgrade		= 15,

        /// <summary>
        /// 
        /// </summary>
        Skill		= 16,

        /// <summary>
        /// 
        /// </summary>
        Warp		= 17
    }

    /// <summary>
    /// The data for an item.
    /// </summary>
    public abstract class Item
    {
        private ItemType _type;
        private string _version;
        private int _id;
        private string _name;
        private string _category;
        private string _skillLogic;
        private string _description;
        private float _weight;
        private int _buyPrice;
        private int _probability;
        private bool _dropable;
        private int _key;
        private int _recommendedQuantity;
        private int _maxAllowed;
        private PickupMode _pickupMode;
        private int _sellPrice;
        private int _timer;
        private Graphic _icon;

        /// <summary>
        /// Item type of the item.
        /// </summary>
        public ItemType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// The version the item was saved under.
        /// </summary>
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        /// <summary>
        /// Unique identifier for the item.
        /// </summary>
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Name of the item.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The name of the category the item is in.
        /// </summary>
        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }

        /// <summary>
        /// The skill requirements to use the item.
        /// </summary>
        public string SkillLogic
        {
            get { return _skillLogic; }
            set { _skillLogic = value; }
        }

        /// <summary>
        /// Description of the item.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// The weight of the item.
        /// </summary>
        /// <remarks>Truncated to three decimal places.</remarks>
        public float Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        /// <summary>
        /// The cost of the item in the store.
        /// </summary>
        public int BuyPrice
        {
            get { return _buyPrice; }
            set { _buyPrice = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Use -1 to never hide or allow in inventory, and 0 to hide.
        /// </remarks>
        public int Probability
        {
            get { return _probability; }
            set { _probability = value; }
        }

        /// <summary>
        /// If the item is allowed to be dropped.
        /// </summary>
        public bool Dropable
        {
            get { return _dropable; }
            set { _dropable = value; }
        }

        /// <summary>
        /// The default key the item is set to.
        /// </summary>
        public int KeyPreference
        {
            get { return _key; }
            set { _key = value; }
        }

        /// <summary>
        /// The recommended amount of the item.
        /// </summary>
        /// <remarks>Used for auto dumping.</remarks>
        public int RecommendedQuantity
        {
            get { return _recommendedQuantity; }
            set { _recommendedQuantity = value; }
        }

        /// <summary>
        /// The maximum number of this item allowed in a player's inventory.
        /// </summary>
        /// <remarks>
        /// Use 0 for unlimited, negative numbers for absolute limit, and positive
        /// numbers for usable limit.
        /// </remarks>
        public int MaxAllowed
        {
            get { return _maxAllowed; }
            set { _maxAllowed = value; }
        }

        /// <summary>
        /// How the item is picked up.
        /// </summary>
        public PickupMode PickupMode
        {
            get { return _pickupMode; }
            set { _pickupMode = value; }
        }

        /// <summary>
        /// The price of the item when sold in the store.
        /// </summary>
        /// <remarks>
        /// Use -1 to make the item unsaleable, 0 to be saleable but worth nothing.
        /// The item must be purchasable to be able to be sold.
        /// </remarks>
        public int SellPrice
        {
            get { return _sellPrice; }
            set { _sellPrice = value; }
        }

        /// <summary>
        /// How long the item will last in your inventory.
        /// </summary>
        /// <remarks>If the item is dropped it will disappear.</remarks>
        public int ExpireTimer
        {
            get { return _timer; }
            set { _timer = value; }
        }

        /// <summary>
        /// The graphic data for the item's icon.
        /// </summary>
        public Graphic Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }

        /// <summary>
        /// Parses a comma seperated string into an array.
        /// </summary>
        /// <remarks>
        /// Values inside double quotes can have anything in them. The exception
        /// is new line characters which should not be in anywhere in the line.
        /// </remarks>
        /// <param name="line">A line from an item file.</param>
        /// <returns>An array of each comma seperated value or null if it was a comment.</returns>
        public static string[] ParseLine(string line)
        {
            string pattern;
            string[] data;

            // The line will be commented then.
            if (line.StartsWith("-"))
                return null;

            // ,(?=(?:[^"]*"[^"]*")*(?![^"]*"))";
            pattern = ",\\s*(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";

            data = Regex.Split(line, pattern, RegexOptions.None);

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = data[i].Trim();

                if (data[i].StartsWith("\"") && data[i].EndsWith("\""))
                    data[i] = data[i].Substring(1, data[i].Length - 2);
            }

            return data;
        }

        /// <summary>
        /// Parses a comma seperated string into the values of the item.
        /// </summary>
        /// <remarks>Uses the functionality of the <see cref="Item.ParseLine"/></remarks>
        /// <param name="line">A line from an item file.</param>
        public virtual void ParseItem(string line)
        {
            string[] data;
            Graphic icon;

            data = Item.ParseLine(line);

            this.Type = (ItemType)Int32.Parse(data[0]);
            this.Version = data[1];
            this.ID = Int32.Parse(data[2]);
            this.Name = data[3];
            this.Category = data[4];
            this.SkillLogic = data[5];
            this.Description = data[6];

            this.Weight = (float)(Int32.Parse(data[7]) / 1000.0);
            this.BuyPrice = Int32.Parse(data[8]);
            this.Probability = Int32.Parse(data[9]);
            this.Dropable = Boolean.Parse(data[10]);
            this.KeyPreference = Int32.Parse(data[11]);
            this.RecommendedQuantity = Int32.Parse(data[12]);
            this.MaxAllowed = Int32.Parse(data[13]);
            this.PickupMode = (PickupMode)Int32.Parse(data[14]);
            this.SellPrice = Int32.Parse(data[15]);
            this.ExpireTimer = Int32.Parse(data[16]);

            icon = new Graphic();
            icon.BlobFilename = data[17];
            icon.Filename = data[18];
            icon.LightPermuation = Int32.Parse(data[19]);
            icon.PaletteOffset = Int32.Parse(data[20]);
            icon.Hue = Int32.Parse(data[21]);
            icon.Saturation = Int32.Parse(data[22]);
            icon.Value = Int32.Parse(data[23]);
            icon.AnimationTime = Int32.Parse(data[24]);
            this.Icon = icon;
        }
    }
}
