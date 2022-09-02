using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoneDiffinitionOfRoom : MonoBehaviour
{
    [SerializeField] public bool IsEditMode = false;
    [SerializeField] bool useCustomWalls = false;

    [SerializeField] GameObject colliderMaterial = null;
    [SerializeField] float margin = 1f;

    [Range(1, 200)]
    [SerializeField] float roomWidth = 0;
    [Range(1, 200)]
    [SerializeField] float roomHeight = 0;
    [Range(1, 200)]
    [SerializeField] float roomLength = 0;
    [Range(0.1f, 5f)]
    [SerializeField] float wallThickness = 1;

    [SerializeField] GameObject roomPivot_X;
    [SerializeField] GameObject roomPivot_Y;
    [SerializeField] GameObject roomPivot_Z;

    [SerializeField] Vector3 roomScale = new Vector3(0, 0, 0);
    [SerializeField] public Vector3 primitiveCoordinate = new Vector3(0, 0, 0);

    Vector3 colliderMaterialsPosition_X0 = new Vector3(0, 0, 0);
    Vector3 colliderMaterialsPosition_X1 = new Vector3(0, 0, 0);
    Vector3 colliderMaterialsPosition_Y0 = new Vector3(0, 0, 0);
    Vector3 colliderMaterialsPosition_Y1 = new Vector3(0, 0, 0);
    Vector3 colliderMaterialsPosition_Z0 = new Vector3(0, 0, 0);
    Vector3 colliderMaterialsPosition_Z1 = new Vector3(0, 0, 0);
    float colliderMaterialsWidth;
    float colliderMaterialsHeight;
    float colliderMaterialsLength;

    private bool ready = false;

    private void Awake()
    {
        StartCoroutine(Init());
    }


    private IEnumerator Init()
    {
        //yield return new WaitUntil(() => RoomDoorWay.instance.Ready());
        yield return new WaitForSeconds(0f);
        if (useCustomWalls)
        {
            SetWalls();
            MediateInitWalls();
        }
        ScalePosition();
        DeployRoomCore();  //RoomCore を部屋の中心に配置
        ScalePosition();  //RoomCore を中心に配置しなおしてからからもう一度Rayを飛ばすことで、ColliderMaterial を配置するポイントを確定する。
        CreateColliderMaterials();
        InitColliderMaterials();
        if (!IsEditMode)
        {
            GameObject.FindGameObjectWithTag("Room").GetComponent<MeshCollider>().enabled = false;
            this.GetComponent<Renderer>().enabled = false;
            roomPivot_X.GetComponent<Renderer>().enabled = false;
            roomPivot_Y.GetComponent<Renderer>().enabled = false;
            roomPivot_Z.GetComponent<Renderer>().enabled = false;
        }

        ready = true;
    }

    private void FixedUpdate()
    {
        if (!ready) return;
        //Observation();
        Edit();
    }


    private void Edit()
    {
        if (!IsEditMode) return;
        if (useCustomWalls)
        {
            MediateInitWalls();
        }
        ScalePosition();
        InitColliderMaterials();
    }


    private void ScalePosition()
    {
        float distance = 100;
        float duration = 0.02f;

        LayerMask layerMask_Room = 1 << LayerMask.NameToLayer("Room");

        Ray rayX0 = new Ray(transform.position, -(roomPivot_X.transform.position - transform.position));
        Debug.DrawRay(rayX0.origin, rayX0.direction * distance, Color.red, duration, false);
        Physics.Raycast(rayX0, out RaycastHit hitInfoX0, 300, layerMask_Room);

        Ray rayX1 = new Ray(transform.position, roomPivot_X.transform.position - transform.position);
        Debug.DrawRay(rayX1.origin, rayX1.direction * distance, Color.red, duration, false);
        Physics.Raycast(rayX1, out RaycastHit hitInfoX1, 300, layerMask_Room);

        Ray rayY0 = new Ray(transform.position, -(roomPivot_Y.transform.position - transform.position));
        Debug.DrawRay(rayY0.origin, rayY0.direction * distance, Color.green, duration, false);
        Physics.Raycast(rayY0, out RaycastHit hitInfoY0, 300, layerMask_Room);

        Ray rayY1 = new Ray(transform.position, roomPivot_Y.transform.position - transform.position);
        Debug.DrawRay(rayY1.origin, rayY1.direction * distance, Color.green, duration, false);
        Physics.Raycast(rayY1, out RaycastHit hitInfoY1, 300, layerMask_Room);

        Ray rayZ0 = new Ray(transform.position, -(roomPivot_Z.transform.position - transform.position));
        Debug.DrawRay(rayZ0.origin, rayZ0.direction * distance, Color.blue, duration, false);
        Physics.Raycast(rayZ0, out RaycastHit hitInfoZ0, 300, layerMask_Room);

        Ray rayZ1 = new Ray(transform.position, roomPivot_Z.transform.position - transform.position);
        Debug.DrawRay(rayZ1.origin, rayZ1.direction * distance, Color.blue, duration, false);
        Physics.Raycast(rayZ1, out RaycastHit hitInfoZ1, 300, layerMask_Room);

        roomWidth = hitInfoX1.point.x - hitInfoX0.point.x + margin;
        roomHeight = hitInfoY1.point.y - hitInfoY0.point.y + margin;
        roomLength = hitInfoZ1.point.z - hitInfoZ0.point.z + margin;

        roomScale = new Vector3(roomWidth, roomHeight, roomLength);

        //colliderMaterialsPosition_X0 = hitInfoX0.point;
        //colliderMaterialsPosition_X1 = hitInfoX1.point;
        //colliderMaterialsPosition_Y0 = hitInfoY0.point;
        //colliderMaterialsPosition_Y1 = hitInfoY1.point;
        //colliderMaterialsPosition_Z0 = hitInfoZ0.point;
        //colliderMaterialsPosition_Z1 = hitInfoZ1.point;

        colliderMaterialsPosition_X0 = hitInfoX0.point;
        colliderMaterialsPosition_X1 = hitInfoX1.point;
        colliderMaterialsPosition_Y0 = hitInfoY0.point;
        colliderMaterialsPosition_Y1 = hitInfoY1.point;
        colliderMaterialsPosition_Z0 = hitInfoZ0.point;
        colliderMaterialsPosition_Z1 = hitInfoZ1.point;

        colliderMaterialsWidth = roomWidth + margin;
        colliderMaterialsHeight = roomHeight + margin;
        colliderMaterialsLength = roomLength + margin;

        MediateCalculatePrimitiveCoordinate(hitInfoX1, hitInfoY1, hitInfoZ1);
    }

    private void DeployRoomCore()
    {
        transform.position = new Vector3
       (
           colliderMaterialsPosition_X0.x + (colliderMaterialsWidth) / 2,
           colliderMaterialsPosition_Y0.y + (colliderMaterialsHeight) / 2,
           colliderMaterialsPosition_Z0.z + (colliderMaterialsLength) / 2
       );
    }

    [SerializeField] public GameObject collider_Left = null;
    [SerializeField] public GameObject collider_Right = null;
    [SerializeField] public GameObject collider_Floor = null;
    [SerializeField] public GameObject collider_Ceiling = null;
    [SerializeField] public GameObject collider_Back = null;
    [SerializeField] public GameObject collider_Front = null;
    private void CreateColliderMaterials()
    {
        collider_Left = Instantiate(colliderMaterial);
        collider_Right = Instantiate(colliderMaterial);
        collider_Floor = Instantiate(colliderMaterial);
        collider_Ceiling = Instantiate(colliderMaterial);
        collider_Back = Instantiate(colliderMaterial);
        collider_Front = Instantiate(colliderMaterial);

        collider_Left.name = "Collider_Left";
        collider_Right.name = "Collider_Right";
        collider_Floor.name = "Collider_Floor";
        collider_Ceiling.name = "Collider_Ceiling";
        collider_Back.name = "Collider_Back";
        collider_Front.name = "Collider_Front";

        collider_Left.GetComponent<WallType>().type = WallTypes.LeftWall;
        collider_Right.GetComponent<WallType>().type = WallTypes.RightWall;
        collider_Floor.GetComponent<WallType>().type = WallTypes.Floor;
        collider_Ceiling.GetComponent<WallType>().type = WallTypes.Ceiling;
        collider_Back.GetComponent<WallType>().type = WallTypes.BackWall;
        collider_Front.GetComponent<WallType>().type = WallTypes.FrontWall;
    }
    private void InitColliderMaterials()
    {
        collider_Left.transform.position = colliderMaterialsPosition_X0 - new Vector3(wallThickness / 2 - 0.05f + margin, 0, 0);
        collider_Right.transform.position = colliderMaterialsPosition_X1 + new Vector3(wallThickness / 2 - 0.05f + margin, 0, 0);
        collider_Floor.transform.position = colliderMaterialsPosition_Y0 - new Vector3(0, wallThickness / 2 - 0.05f + margin, 0);
        collider_Ceiling.transform.position = colliderMaterialsPosition_Y1 + new Vector3(0, wallThickness / 2 - 0.05f + margin, 0);
        collider_Back.transform.position = colliderMaterialsPosition_Z0 - new Vector3(0, 0, wallThickness / 2 - 0.05f + margin);
        collider_Front.transform.position = colliderMaterialsPosition_Z1 + new Vector3(0, 0, wallThickness / 2 - 0.05f + margin);

        collider_Left.transform.localScale = new Vector3(wallThickness, colliderMaterialsHeight, colliderMaterialsLength);
        collider_Right.transform.localScale = new Vector3(wallThickness, colliderMaterialsHeight, colliderMaterialsLength);
        collider_Floor.transform.localScale = new Vector3(colliderMaterialsWidth, wallThickness, colliderMaterialsLength);
        collider_Ceiling.transform.localScale = new Vector3(colliderMaterialsWidth, wallThickness, colliderMaterialsLength);
        collider_Back.transform.localScale = new Vector3(colliderMaterialsWidth, colliderMaterialsHeight, wallThickness);
        collider_Front.transform.localScale = new Vector3(colliderMaterialsWidth, colliderMaterialsHeight, wallThickness);
    }

    private void MediateCalculatePrimitiveCoordinate(RaycastHit hitInfoX1, RaycastHit hitInfoY1, RaycastHit hitInfoZ1)
    {
        float primitiveCoordinate_X = 0f;
        float primitiveCoordinate_Y = 0f;
        float primitiveCoordinate_Z = 0f;

        if (hitInfoX1.collider.gameObject.tag == "Room")
        {
            primitiveCoordinate_X = CalculatePrimitiveCoordinate(hitInfoX1.point).x;
        }
        if (hitInfoY1.collider.gameObject.tag == "Room")
        {
            primitiveCoordinate_Y = CalculatePrimitiveCoordinate(hitInfoY1.point).y;
        }
        if (hitInfoZ1.collider.gameObject.tag == "Room")
        {
            primitiveCoordinate_Z = CalculatePrimitiveCoordinate(hitInfoZ1.point).z;
        }

        primitiveCoordinate = new Vector3(primitiveCoordinate_X, primitiveCoordinate_Y, primitiveCoordinate_Z);
    }
    private Vector3 CalculatePrimitiveCoordinate(Vector3 hitPoint)
    {
        float primithiveMagnitude = ((hitPoint - transform.position).magnitude + margin * 2) / 10;
        Vector3 direction = (hitPoint - transform.position).normalized;
        Vector3 primitiveCoordinate = primithiveMagnitude * direction;
        return primitiveCoordinate;
    }

    public Vector3 CoordinateFromPosition(Vector3 worldPisition)
    {
        Vector3 Coordinate = new Vector3
        (
            (worldPisition.x - transform.position.x) / primitiveCoordinate.x,
            (worldPisition.y - transform.position.y) / primitiveCoordinate.y,
            (worldPisition.z - transform.position.z) / primitiveCoordinate.z
        );
        return Coordinate;
    }

    public Vector3 PositionFromCoordinate(Vector3 coordinate)
    {
        Vector3 worldPosition = new Vector3
        (
            coordinate.x * primitiveCoordinate.x + transform.position.x,
            coordinate.y * primitiveCoordinate.y + transform.position.y,
            coordinate.z * primitiveCoordinate.z + transform.position.z
        );
        return worldPosition;
    }


    //useCustomWalls がONの時だけ使う＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊
    //＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊
    private Dictionary<string, GameObject> wall = new Dictionary<string, GameObject> { };

    private void SetWalls()
    {
        wall.Add("Left", GameObject.Find("LeftWall"));
        wall.Add("Right", GameObject.Find("RightWall"));
        wall.Add("Floor", GameObject.Find("Floor"));
        wall.Add("Ceiling", GameObject.Find("Ceiling"));
        wall.Add("Back", GameObject.Find("BackWall"));
        wall.Add("Front", GameObject.Find("FrontWall"));
    }
    private void MediateInitWalls()
    {
        InitWalls(wall["Left"]);
        InitWalls(wall["Right"]);
        InitWalls(wall["Floor"]);
        InitWalls(wall["Ceiling"]);
        InitWalls(wall["Back"]);
        InitWalls(wall["Front"]);
    }
    private void InitWalls(GameObject ob)
    {
        //roomScale = new Vector3(roomWidth, roomHeight, roomLength);

        float fixedRoomWidth = roomScale.x / 2 + 0.5f;
        float fixedRoomHeight = roomScale.y / 2 + 0.5f;
        float fixedRoomLength = roomScale.z / 2 + 0.5f;

        ob.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (ob == wall["Left"] || ob == wall["Right"])
        {
            ob.transform.localScale = new Vector3(wallThickness, roomScale.y, roomScale.z);
            //KeepLossyScale(ob);
            if (ob == wall["Left"])
            {
                ob.transform.position = new Vector3(-fixedRoomWidth, 0, 0);
                return;
            }
            else
            {
                ob.transform.position = new Vector3(fixedRoomWidth, 0, 0);
                return;
            }
        }
        if (ob == wall["Floor"] || ob == wall["Ceiling"])
        {
            ob.transform.localScale = new Vector3(roomScale.x, wallThickness, roomScale.z);
            //KeepLossyScale(ob);
            if (ob == wall["Floor"])
            {
                ob.transform.position = new Vector3(0, -fixedRoomHeight, 0);
                return;
            }
            else
            {
                ob.transform.position = new Vector3(0, fixedRoomHeight, 0);
                return;
            }
        }
        if (ob == wall["Back"] || ob == wall["Front"])
        {
            ob.transform.localScale = new Vector3(roomScale.x, roomScale.y, wallThickness);
            //KeepLossyScale(ob);
            if (ob == wall["Back"])
            {
                ob.transform.position = new Vector3(0, 0, -fixedRoomLength);
                return;
            }
            else
            {
                ob.transform.position = new Vector3(0, 0, fixedRoomLength);
                return;
            }
        }
    }
    //＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊
    //＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊




    //確認用＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊
    //[SerializeField] GameObject target1;
    //[SerializeField] GameObject target2;
    //[SerializeField] GameObject target3;
    //[SerializeField] Vector3 targetPosition1 = new Vector3(0, 0, 0);
    //[SerializeField] Vector3 targetPosition2 = new Vector3(0, 0, 0);
    //[SerializeField] Vector3 targetPosition3 = new Vector3(0, 0, 0);
    //private void Observation()
    //{
    //    if (target1) targetPosition1 = target1.transform.position;
    //    if (target2) targetPosition2 = target2.transform.position;
    //    if (target3) targetPosition3 = target3.transform.position;
    //}
    //＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊
}
