using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class EntityModel
{
    public int ID;
    public int HP;

    public EntityModel(int id, int hp)
    {
        this.ID = id;
        this.HP = hp;
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("ID:" + ID);
        builder.Append("/HP:" + HP.ToString());
        return builder.ToString();
    }
}

public class EntityTable : ScriptableObject
{
    public List<Sheet> sheets = new List<Sheet>();

    [System.SerializableAttribute]
    public class Sheet
    {
        public string name = string.Empty;
        public List<Param> list = new List<Param>();
    }

    [System.SerializableAttribute]
    public class Param
    {
        public int ID;
        public int HP;

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("Param ID : " + ID);
            builder.Append("/ HP : " + HP);

            return builder.ToString();
        }
    }
}