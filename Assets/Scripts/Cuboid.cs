using UnityEngine;

public class Cuboid : MonoBehaviour
{
    
    public float speed = -10.0f;
    public bool drop;
    public float width;
    public float height = 1.5f;
    public float depth;

    private const float MAX_MOVE_X = 10.0f;
    private const float MAX_MOVE_Z = 10.0f;
    private bool moveX;
    private float offsetY;
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
        PerfectDrop perfectDrop = FindObjectOfType<PerfectDrop>();
        Cuboid prevCuboid = cuboidGenerator.previousCuboid;
        Vector3 prevCuboidPos = prevCuboid.transform.position;

        if (moveX)
        {
            float difX = transform.position.x - prevCuboidPos.x;
            float remainWidth, lostWidth, remainX, lostX;

            lostWidth = Mathf.Abs(difX);
            remainWidth = prevCuboid.width - lostWidth;

            if(difX > GameManager.GAME_DIFICULTY) // lost at x > 0
            {
                if (difX > prevCuboid.width) return false;

                remainX = prevCuboidPos.x + prevCuboid.width / 2 - remainWidth / 2;
                lostX = prevCuboidPos.x + prevCuboid.width / 2 + lostWidth / 2;

                //Generate Remain Cuboid
                transform.position = Vector3.zero;
                SetUp(remainWidth, depth, offsetY, moveX, height);
                Generate(false);
                transform.position = new Vector3(remainX, offsetY, prevCuboidPos.z);

                //Generate Lost Cuboid
                Cuboid lostCuboid = Instantiate<Cuboid>(cuboidGenerator.cuboidPrefab);
                lostCuboid.transform.position = new Vector3(lostX, offsetY, transform.position.z);
                lostCuboid.SetUp(lostWidth, depth, offsetY, moveX, height);
                lostCuboid.Generate(true);
                lostCuboid.rb.freezeRotation = false;
                lostCuboid.Drop();
            }
            else if (difX < -GameManager.GAME_DIFICULTY)
            { //lost at x < 0

                if (difX < -prevCuboid.width) return false;

                remainX = prevCuboidPos.x - prevCuboid.width / 2 + remainWidth / 2; 
                lostX = prevCuboidPos.x - prevCuboid.width / 2 - lostWidth / 2;


                transform.position = Vector3.zero;
                SetUp(remainWidth, depth, offsetY, moveX, height);
                Generate(false);
                transform.position = new Vector3(remainX, offsetY, prevCuboidPos.z);

                //Generate Lost Cuboid
                Cuboid lostCuboid = Instantiate<Cuboid>(cuboidGenerator.cuboidPrefab);
                lostCuboid.transform.position = new Vector3(lostX, offsetY, transform.position.z);
                lostCuboid.SetUp(lostWidth, depth, offsetY, moveX, height);
                lostCuboid.Generate(true);
                lostCuboid.rb.freezeRotation = false;
                lostCuboid.Drop();
            }
            else
            {
                //perfect drop
                Debug.Log("perfect drop");
                transform.position = Vector3.zero;
                SetUp(width, depth, offsetY, moveX, height);
                Generate(false);
                transform.position = new Vector3(prevCuboidPos.x, offsetY, prevCuboidPos.z);

                Vector3 currentPos = transform.position;
                Vector3 p0 = new Vector3(currentPos.x + width / 2 + 0.5f, offsetY + height / 2, currentPos.z + depth / 2 + 0.5f);
                Vector3 p1 = new Vector3(currentPos.x + width / 2 + 0.5f, offsetY + height / 2, currentPos.z -(depth / 2 + 0.5f));
                Vector3 p2 = new Vector3(currentPos.x + -(width / 2 + 0.5f), offsetY + height / 2, currentPos.z -(depth / 2 + 0.5f));
                Vector3 p3 = new Vector3(currentPos.x + -(width / 2 + 0.5f), offsetY + height / 2, currentPos.z + depth / 2 + 0.5f);
                //Do animation
                perfectDrop.SetPoints(p0, p1, p2, p3);
                perfectDrop.StartAnim();

            }
        }
        else
        {
            float difZ = transform.position.z - prevCuboidPos.z;
            float remainDepth, lostDepth, remainZ, lostZ;

            lostDepth = Mathf.Abs(difZ);
            remainDepth = prevCuboid.depth - lostDepth;

            if (difZ > GameManager.GAME_DIFICULTY) // lost at z > 0
            {
                //if (difZ > prevCuboid.height) return false;

                remainZ = prevCuboidPos.z + prevCuboid.depth / 2 - remainDepth / 2;
                lostZ = prevCuboidPos.z + prevCuboid.depth / 2 + lostDepth / 2;
                transform.position = Vector3.zero;
                SetUp(width, remainDepth, offsetY, moveX, height);
                Generate(false);
                transform.position = new Vector3(prevCuboidPos.x, offsetY, remainZ);

                //Generate Lost Cuboid
                Cuboid lostCuboid = Instantiate<Cuboid>(cuboidGenerator.cuboidPrefab);
                lostCuboid.transform.position = new Vector3(transform.position.x, offsetY, lostZ);
                lostCuboid.SetUp(width, lostDepth, offsetY, moveX, height);
                lostCuboid.Generate(true);
                lostCuboid.rb.freezeRotation = false;
                lostCuboid.Drop();
            }
            else if (difZ < -GameManager.GAME_DIFICULTY)
            { //lost at z < 0

                //if (difZ < -prevCuboid.height) return false;

                remainZ = prevCuboidPos.z - prevCuboid.depth / 2 + remainDepth / 2;
                lostZ = prevCuboidPos.z - prevCuboid.depth / 2 - lostDepth / 2;
                transform.position = Vector3.zero;
                SetUp(width, remainDepth, offsetY, moveX, height);
                Generate(false);
                transform.position = new Vector3(prevCuboidPos.x, offsetY, remainZ);

                //Generate Lost Cuboid
                Cuboid lostCuboid = Instantiate<Cuboid>(cuboidGenerator.cuboidPrefab);
                lostCuboid.transform.position = new Vector3(transform.position.x, offsetY, lostZ);
                lostCuboid.SetUp(width, lostDepth, offsetY, moveX, height);
                lostCuboid.Generate(true);
                lostCuboid.rb.freezeRotation = false;
                lostCuboid.Drop();
            }
            else
            {
                //perfect drop
                Debug.Log("perfect drop");
                transform.position = Vector3.zero;
                SetUp(width, depth, offsetY, moveX, height);
                Generate(false);
                transform.position = new Vector3(prevCuboidPos.x, offsetY, prevCuboidPos.z);

                Vector3 currentPos = transform.position;
                Vector3 p0 = new Vector3(currentPos.x + width / 2 + 0.5f, offsetY + height / 2, currentPos.z + depth / 2 + 0.5f);
                Vector3 p1 = new Vector3(currentPos.x + width / 2 + 0.5f, offsetY + height / 2, currentPos.z - (depth / 2 + 0.5f));
                Vector3 p2 = new Vector3(currentPos.x + -(width / 2 + 0.5f), offsetY + height / 2, currentPos.z - (depth / 2 + 0.5f));
                Vector3 p3 = new Vector3(currentPos.x + -(width / 2 + 0.5f), offsetY + height / 2, currentPos.z + depth / 2 + 0.5f);
                //Do animation
                perfectDrop.SetPoints(p0, p1, p2, p3);
                perfectDrop.StartAnim();


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
        collider.size = new Vector3(width, height, depth);
        collider.center = new Vector3(0, height/2, 0);
    }

    public void SetUp(float width, float depth, float offsetY, bool moveX, float height)
    { 
        this.width = width;
        this.depth = depth;
        this.offsetY = offsetY;
        this.moveX = moveX;
        this.height = height;
    }

    public void MoveToMaxMove()
    {
        //Move cuboid to starting moving position;
        if (moveX) transform.position = new Vector3(MAX_MOVE_X, offsetY, transform.position.z);
        else transform.position = new Vector3(transform.position.x, offsetY, MAX_MOVE_Z);
    }

    private void SetVertices()
    {
        Vector3 vertice_0 = new Vector3(-width/2, 0, depth / 2);
        Vector3 vertice_1 = new Vector3(width/2, 0, depth / 2);
        Vector3 vertice_2 = new Vector3(width/2, 0, -depth / 2);
        Vector3 vertice_3 = new Vector3(-width / 2, 0, -depth / 2);
        Vector3 vertice_4 = new Vector3(-width / 2, height, depth / 2);
        Vector3 vertice_5 = new Vector3(width/2, height, depth / 2);
        Vector3 vertice_6 = new Vector3(width/2, height, -depth / 2);
        Vector3 vertice_7 = new Vector3(-width / 2, height, -depth / 2);
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
