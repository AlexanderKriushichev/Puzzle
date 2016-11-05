﻿public class LineBonus : Bonus {

    private TypeLineBonus type;

    public LineBonus(TypeLineBonus _type, Field _field, Crystal _crystal)
        : base(_field, _crystal)
    {
        type = _type;
    }

    public override void Acivate()
    {
        switch (type)
        {
            case TypeLineBonus.Horizontal:
                {
                    for (int i = 0; i < Field.size; i++)
                    {
                        field.cells[i, crystal.cell.y].Destroy();
                    }
                    break;
                }
            case TypeLineBonus.Verical:
                {
                    for (int i = 0; i < Field.size; i++)
                    {
                        field.cells[crystal.cell.x, i].Destroy();
                    }
                    break;
                }
        }
    }

}

public enum TypeLineBonus {Horizontal, Verical}