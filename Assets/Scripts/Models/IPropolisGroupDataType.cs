using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Propolis
{
    public interface IPropolisGroupDataType
    {
        List<IPropolisDataType> Childrens { get; set; }
    }
}



