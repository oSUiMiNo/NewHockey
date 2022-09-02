//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class DivisionPointsPool : MonoBehaviour
//{
//    private ZoneDiffinitionOfRoom zone = null;
//    private int coordinateDivision = 0;
//    private Vector3 primitiveCoordinate = Vector3.zero;
//    [SerializeField] private GameObject divisionPoint = null;
//    [SerializeField] private List<GameObject> pool_X = new List<GameObject> { };
//    [SerializeField] private List<GameObject> pool_Y = new List<GameObject> { };
//    [SerializeField] private List<GameObject> pool_Z = new List<GameObject> { };
//    private void Start()
//    {
//        zone = GameObject.Find("Room").GetComponent<ZoneDiffinitionOfRoom>();
//        coordinateDivision = zone.coordinateDivision;
//        primitiveCoordinate = zone.primitiveCoordinate;

//        for (int i = 0; i < coordinateDivision; i++)
//        {
//            GameObject newDivision_X = Instantiate(divisionPoint, new Vector3(primitiveCoordinate.x * i, 0, 0), Quaternion.identity);
//            GameObject newDivision_Y = Instantiate(divisionPoint, new Vector3(0, primitiveCoordinate.y * i, 0), Quaternion.identity);
//            GameObject newDivision_Z = Instantiate(divisionPoint, new Vector3(0, 0, primitiveCoordinate.z * i), Quaternion.identity);
//            newDivision_X.SetActive(false);
//            newDivision_Y.SetActive(false);
//            newDivision_Z.SetActive(false);
//            pool_X.Add(newDivision_X);
//            pool_Y.Add(newDivision_Y);
//            pool_Z.Add(newDivision_Z);
//        }
//    }

//    private void FixedUpdate()
//    {
//        if (zone.IsEditMode)
//        {
//            for (int i = 0; i < coordinateDivision; i++)
//                foreach (var item in pool_X) item.SetActive(true);
//            foreach (var item in pool_Y) item.SetActive(true);
//            foreach (var item in pool_Z) item.SetActive(true);
//        }
//    }

//    private GameObject GetObject(string objName)
//    {
//        if (objName == "divisionPoint_X")
//        {
//            foreach (var item in pool_X)
//            {
//                if (item.activeSelf == false)
//                {
//                    item.SetActive(true);
//                    return item;
//                }
//                return Instantiate(divisionPoint);
//            }
//        }
//if (objName == "divisionPoint_Y")
//{
//    foreach (var item in pool_Y)
//    {
//        if (item.activeSelf == false)
//        {
//            item.SetActive(true);
//            return item;
//        }
//        return Instantiate(divisionPoint);
//    }
//}
//if (objName == "divisionPoint_Z")
//{
//    foreach (var item in pool_Z)
//    {
//        if (item.activeSelf == false)
//        {
//            item.SetActive(true);
//            return item;
//        }
//        return Instantiate(divisionPoint);
//    }
//}
//return null;
//    }
//}
