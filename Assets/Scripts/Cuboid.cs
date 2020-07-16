using UnityEngine;

public class Cuboid : MonoBehaviour
{
    public float speed = -10.0f;
    public bool drop;
    public float width;
    public float height;

    private const float MAX_MOVE_X = 10.0f;
    private const float MAX_MOVE_Z = 10.0f;
    private bool moveX;
    private int offsetY;
    private Mesh mesh;
    private Vector3[] vertices;
    private Rigidbody rb;
    private BoxCollider collider;

    //this never changes
    private int[] triangles = {
        // Cube Bottom Side Triangles
        3, 1, 0,
        3, 2, 1,    
        // Cube Left Side Triangles
        3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
        3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
        // Cube Front Side Triangles
        3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
        3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
        // Cube Back Side Triangles
        3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
        3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
        // Cube Rigth Side Triangles
        3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
        3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
        // Cube Top Side Triangles
        3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
        3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
    };
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<BoxCollider>();
    }

    void FixedUpdate()
    {
        if (drop) return;

        Vector3 nextPos;
        if (moveX)
        {
            if (transform.position.x + speed * Time.fixedDeltaTime > MAX_MOVE_X ||
                transform.position.x + speed * Time.fixedDeltaTime < -MAX_MOVE_X)
                speed = -speed;

            nextPos = transform.position + new Vector3(speed * Time.fixedDeltaTime, 0, 0);
        }
        else
        {
            if (transform.position.z + speed * Time.fixedDeltaTime > MAX_MOVE_Z ||
                transform.position.z + speed * Time.fixedDeltaTime < -MAX_MOVE_Z)
                speed = -speed;

            nextPos = transform.position + new Vector3(0, 0, speed * Time.fixedDeltaTime);
        }
        

        rb.MovePosition(nextPos);
    }

    public void Drop()
    {
        drop = true;
        rb.useGravity = true;
    }


    public bool Separate()
    {
        CuboidGenerator cuboidGenerator = FindObjectOfType<CuboidGenerator>();
        Cuboid prevCuboid = cuboidGenerator.previousCuboid;
        Vector3 prevCuboidPos = prevCuboid.transform.position;

        if (moveX)
        {
            float difX = transform.position.x - prevCuboidPos.x;
            float remainWidth, lostWidth, remainX, lostX;

            lostWidth = Mathf.Abs(difX);
            remainWidth = prevCuboid.width - lostWidth;

            if(difX > 0) // lost at x > 0
            {
                if (difX > prevCuboid.width) return false;

                remainX = prevCuboidPos.x + prevCuboid.width / 2 - remainWidth / 2;
                lostX = prevCuboidPos.x + prevCuboid.width / 2 + lostWidth / 2;

                //Generate Remain Cuboid
                transform.position = Vector3.zero;
                SetUp(remainWidth, height, offsetY, moveX);
                Generate(false);
                transform.position = new Vector3(remainX, offsetY, prevCuboidPos.z);

                //Generate Lost Cuboid
                Cuboid lostCuboid = Instantiate<Cuboid>(FindObjectOfType<CuboidGenerator>().cuboidPrefab);
                lostCuboid.transform.position = new Vector3(lostX, offsetY, transform.position.z);
                lostCuboid.SetUp(lostWidth, height, offsetY, moveX);
                lostCuboid.Generate(true);
                lostCuboid.rb.freezeRotation = false;
                lostCuboid.Drop();
            }
            else if (difX < 0){ //lost at x < 0

                if (difX < -prevCuboid.width) return false;

                remainX = prevCuboidPos.x - prevCuboid.width / 2 + remainWidth / 2; 
                lostX = prevCuboidPos.x - prevCuboid.width / 2 - lostWidth / 2;


                transform.position = Vector3.zero;
                SetUp(remainWidth, height, offsetY, moveX);
                Generate(false);
                transform.position = new Vector3(remainX, offsetY, prevCuboidPos.z);

                //Generate Lost Cuboid
                Cuboid lostCuboid = Instantiate<Cuboid>(FindObjectOfType<CuboidGenerator>().cuboidPrefab);
                lostCuboid.transform.position = new Vector3(lostX, offsetY, transform.position.z);
                lostCuboid.SetUp(lostWidth, height, offsetY, moveX);
                lostCuboid.Generate(true);
                lostCuboid.rb.freezeRotation = false;
                lostCuboid.Drop();
            }
            else
            {
                //perfect match
                Debug.Log("perfect match");
            }
        }
        else
        {
            float difZ = transform.position.z - prevCuboidPos.z;
            float remainHeight, lostHeight, remainZ, lostZ;

            lostHeight = Mathf.Abs(difZ);
            remainHeight = prevCuboid.height - lostHeight;

            if (difZ > 0) // lost at z > 0
            {
                //if (difZ > prevCuboid.height) return false;

                remainZ = prevCuboidPos.z + prevCuboid.height / 2 - remainHeight / 2;
                lostZ = prevCuboidPos.z + prevCuboid.height / 2 + lostHeight / 2;
                transform.position = Vector3.zero;
                SetUp(width, remainHeight, offsetY, moveX);
                Generate(false);
                transform.position = new Vector3(prevCuboidPos.x, offsetY, remainZ);

                //Generate Lost Cuboid
                Cuboid lostCuboid = Instantiate<Cuboid>(FindObjectOfType<CuboidGenerator>().cuboidPrefab);
                lostCuboid.transform.position = new Vector3(transform.position.x, offsetY, lostZ);
                lostCuboid.SetUp(width, lostHeight, offsetY, moveX);
                lostCuboid.Generate(true);
                lostCuboid.rb.freezeRotation = false;
                lostCuboid.Drop();
            }
            else if (difZ < 0)
            { //lost at z < 0

                //if (difZ < -prevCuboid.height) return false;

                remainZ = prevCuboidPos.z - prevCuboid.height / 2 + remainHeight / 2;
                lostZ = prevCuboidPos.z - prevCuboid.height / 2 - lostHeight / 2;
                transform.position = Vector3.zero;
                SetUp(width, remainHeight, offsetY, moveX);
                Generate(false);
                transform.position = new Vector3(prevCuboidPos.x, offsetY, remainZ);

                //Generate Lost Cuboid
                Cuboid lostCuboid = Instantiate<Cuboid>(FindObjectOfType<CuboidGenerator>().cuboidPrefab);
                lostCuboid.transform.position = new Vector3(transform.position.x, offsetY, lostZ);
                lostCuboid.SetUp(width, lostHeight, offsetY, moveX);
                lostCuboid.Generate(true);
                lostCuboid.rb.freezeRotation = false;
                lostCuboid.Drop();
            }
            else
            {
                //perfect match
                Debug.Log("perfect match");
            }
        }

        return true;
    }

    public void Generate(bool createMesh)
    {
        if (createMesh)
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();

        mesh.name = "Cuboid";
        mesh.Clear();
        SetVertices();
        SetNormals();
        mesh.triangles = triangles;
        mesh.Optimize();
        collider.size = new Vector3(width, 1, height);
        collider.center = new Vector3(0, 0.5f, 0);
    }

    public void SetUp(float width, float height, int offsetY, bool moveX)
    { 
        this.width = width;
        this.height = height;
        this.offsetY = offsetY;
        this.moveX = moveX;
    }

    public void MoveToMaxMove()
    {
        //Move cuboid to starting moving position;
        if (moveX) transform.position = new Vector3(MAX_MOVE_X, offsetY, transform.position.z);
        else transform.position = new Vector3(transform.position.x, offsetY, MAX_MOVE_Z);
    }

    private void SetVertices()
    {
        Vector3 vertice_0 = new Vector3(-width/2, 0, height/2);
        Vector3 vertice_1 = new Vector3(width/2, 0, height/2);
        Vector3 vertice_2 = new Vector3(width/2, 0, -height / 2);
        Vector3 vertice_3 = new Vector3(-width / 2, 0, -height / 2);
        Vector3 vertice_4 = new Vector3(-width / 2, 1, height/2);
        Vector3 vertice_5 = new Vector3(width/2, 1, height/2);
        Vector3 vertice_6 = new Vector3(width/2, 1, -height / 2);
        Vector3 vertice_7 = new Vector3(-width / 2, 1, -height/2);
        Vector3[] vertices = new Vector3[]
{
            // Bottom Polygon
            vertice_0, vertice_1, vertice_2, vertice_3,
            // Left Polygon
            vertice_7, vertice_4, vertice_0, vertice_3,
            // Front Polygon
            vertice_4, vertice_5, vertice_1, vertice_0,
            // Back Polygon
            vertice_6, vertice_7, vertice_3, vertice_2,
            // Right Polygon
            vertice_5, vertice_6, vertice_2, vertice_1,
            // Top Polygon
            vertice_7, vertice_6, vertice_5, vertice_4
            };

            mesh.vertices = vertices;
    }

    private void SetNormals()
    {
   
        Vector3 up = Vector3.up;
        Vector3 down = Vector3.down;
        Vector3 front = Vector3.forward;
        Vector3 back = Vector3.back;
        Vector3 left = Vector3.left;
        Vector3 right = Vector3.right;

        Vector3[] normals = 
        {
            // Bottom Side Render
                down, down, down, down,
                    
            // LEFT Side Render
                left, left, left, left,
                    
            // FRONT Side Render
                front, front, front, front,
                    
            // BACK Side Render
                back, back, back, back,
                    
            // RIGTH Side Render
                right, right, right, right,
                    
            // UP Side Render
                up, up, up, up
        };

        mesh.normals = normals;
    }

}
