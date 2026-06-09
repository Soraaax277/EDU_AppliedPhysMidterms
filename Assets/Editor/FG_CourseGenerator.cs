using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class FG_CourseGenerator : EditorWindow
{
    [MenuItem("Tools/Generate Fall Guys Course")]
    public static void GenerateCourse()
    {
        Light dirLight = FindObjectOfType<Light>();
        if (dirLight == null)
        {
            GameObject lightObj = new GameObject("Directional Light");
            dirLight = lightObj.AddComponent<Light>();
            dirLight.type = LightType.Directional;
            lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);
        }

        // Materials
        Material matPlayer = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (matPlayer.shader == null) matPlayer = new Material(Shader.Find("Standard"));
        matPlayer.color = Color.cyan;

        Material matPlatform = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (matPlatform.shader == null) matPlatform = new Material(Shader.Find("Standard"));
        matPlatform.color = Color.gray;

        Material matObstacle = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (matObstacle.shader == null) matObstacle = new Material(Shader.Find("Standard"));
        matObstacle.color = Color.magenta;
        
        Material matBouncer = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (matBouncer.shader == null) matBouncer = new Material(Shader.Find("Standard"));
        matBouncer.color = Color.yellow;
        
        Material matWin = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (matWin.shader == null) matWin = new Material(Shader.Find("Standard"));
        matWin.color = Color.green;
        
        Material matCheckpoint = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (matCheckpoint.shader == null) matCheckpoint = new Material(Shader.Find("Standard"));
        matCheckpoint.color = Color.blue;

        Material matJoints = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (matJoints.shader == null) matJoints = new Material(Shader.Find("Standard"));
        matJoints.color = new Color(1f, 0.5f, 0f);

        Material matTrampoline = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (matTrampoline.shader == null) matTrampoline = new Material(Shader.Find("Standard"));
        matTrampoline.color = new Color(0f, 0.8f, 1f);

        Material matFakeFloor = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (matFakeFloor.shader == null) matFakeFloor = new Material(Shader.Find("Standard"));
        matFakeFloor.color = new Color(0.8f, 0.2f, 0.2f);

        // Player
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.position = new Vector3(0, 2, 0);
        player.tag = "Player";
        player.GetComponent<Renderer>().sharedMaterial = matPlayer;
        Rigidbody pRb = player.AddComponent<Rigidbody>();
        pRb.constraints = RigidbodyConstraints.FreezeRotation;
        pRb.mass = 1f;
        pRb.linearDamping = 1f;
        player.AddComponent<FG_PlayerMovement>();

        // Third Person Camera Setup
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.transform.SetParent(null); // Ensure it's not a child
            FG_CameraController camCtrl = mainCam.gameObject.AddComponent<FG_CameraController>();
            camCtrl.target = player.transform;
            camCtrl.distance = 8f;
            camCtrl.height = 4f;
        }

        // UI & GameManager
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject textObj = new GameObject("StatusText");
        textObj.transform.SetParent(canvasObj.transform);
        TMP_Text statusText = textObj.AddComponent<TextMeshProUGUI>();
        statusText.text = "Status";
        statusText.fontSize = 72;
        statusText.alignment = TextAlignmentOptions.Center;
        statusText.color = Color.white;
        RectTransform rt = textObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        textObj.SetActive(false);

        GameObject gmObj = new GameObject("GameManager");
        FG_GameManager gm = gmObj.AddComponent<FG_GameManager>();
        gm.player = player.transform;
        gm.statusText = statusText;

        // COURSE LAYOUT
        Vector3 currentPos = Vector3.zero;

        // 1. Start Platform
        CreatePlatform(currentPos, new Vector3(10, 1, 10), matPlatform);
        currentPos += new Vector3(0, 0, 15);

        // 2. Breakable Walls (FixedJoints)
        CreateCheckpoint(currentPos, matCheckpoint);
        CreatePlatform(currentPos, new Vector3(14, 1, 30), matPlatform);
        for (int row = 0; row < 3; row++)
        {
            Vector3 wallCenter = currentPos + new Vector3(0, 0.5f, -5 + row * 10f);
            
            // Build a wall of blocks
            for (int x = -3; x <= 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    Vector3 blockPos = wallCenter + new Vector3(x * 1.5f, y * 1.5f, 0);
                    GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block.name = "BreakableBlock";
                    block.transform.position = blockPos;
                    block.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
                    block.GetComponent<Renderer>().sharedMaterial = matJoints;
                    
                    Rigidbody bRb = block.AddComponent<Rigidbody>();
                    bRb.mass = 2f;
                    
                    // Attach it to an invisible anchor using FixedJoint
                    GameObject anchor = new GameObject("Anchor");
                    anchor.transform.position = blockPos + new Vector3(0, -1f, 0);
                    Rigidbody aRb = anchor.AddComponent<Rigidbody>();
                    aRb.isKinematic = true;

                    FixedJoint fj = block.AddComponent<FixedJoint>();
                    fj.connectedBody = aRb;
                    fj.breakForce = 250f; // Breaks when player runs into it
                    fj.breakTorque = 250f;
                }
            }
        }
        currentPos += new Vector3(0, 0, 30);

        // 3. Spring Trampolines (SpringJoints)
        CreateCheckpoint(currentPos, matCheckpoint);
        for (int i = 0; i < 5; i++)
        {
            Vector3 trampPos = currentPos + new Vector3(0, 0, i * 8f);
            
            GameObject anchor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            anchor.name = "TrampAnchor";
            anchor.transform.position = trampPos + new Vector3(0, -2f, 0);
            anchor.GetComponent<Renderer>().enabled = false;
            Rigidbody anchorRb = anchor.AddComponent<Rigidbody>();
            anchorRb.isKinematic = true;

            GameObject board = GameObject.CreatePrimitive(PrimitiveType.Cube);
            board.name = "Trampoline";
            board.transform.position = trampPos;
            board.transform.localScale = new Vector3(6, 0.5f, 6);
            board.GetComponent<Renderer>().sharedMaterial = matTrampoline;
            Rigidbody boardRb = board.AddComponent<Rigidbody>();
            boardRb.mass = 5f;
            boardRb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

            SpringJoint spring = board.AddComponent<SpringJoint>();
            spring.connectedBody = anchorRb;
            spring.spring = 200f;
            spring.damper = 5f;
        }
        currentPos += new Vector3(0, 0, 42);

        // 4. Single Spinner Alley (HingeJoint Motors)
        CreateCheckpoint(currentPos, matCheckpoint);
        CreatePlatform(currentPos, new Vector3(12, 1, 35), matPlatform);
        for (int i = 0; i < 3; i++)
        {
            Vector3 spinnerPos = currentPos + new Vector3(0, 1f, -10 + i * 15f);
            
            GameObject anchor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            anchor.name = "SpinnerAnchor";
            anchor.transform.position = spinnerPos + new Vector3(0, -0.5f, 0);
            anchor.transform.localScale = new Vector3(1, 0.5f, 1);
            anchor.GetComponent<Renderer>().sharedMaterial = matPlatform;
            Rigidbody anchorRb = anchor.AddComponent<Rigidbody>();
            anchorRb.isKinematic = true;

            GameObject spinner = GameObject.CreatePrimitive(PrimitiveType.Cube);
            spinner.name = "HingeSpinner";
            spinner.transform.position = spinnerPos + new Vector3(0, 0.2f, 0); 
            spinner.transform.localScale = new Vector3(10, 0.5f, 1);
            spinner.GetComponent<Renderer>().sharedMaterial = matObstacle;
            Rigidbody sRb = spinner.AddComponent<Rigidbody>();
            sRb.mass = 50f; 

            HingeJoint hinge = spinner.AddComponent<HingeJoint>();
            hinge.connectedBody = anchorRb;
            hinge.axis = new Vector3(0, 1, 0);
            hinge.useMotor = true;
            JointMotor motor = hinge.motor;
            motor.targetVelocity = (i % 2 == 0) ? 150f : -150f;
            motor.force = 50000f; 
            motor.freeSpin = false;
            hinge.motor = motor;
        }
        currentPos += new Vector3(0, 0, 35);

        // 5. Massive Seesaw Bridge (HingeJoints)
        CreateCheckpoint(currentPos, matCheckpoint);
        for (int i = 0; i < 5; i++)
        {
            Vector3 seesawPos = currentPos + new Vector3(0, 0, i * 12f);
            
            GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pillar.name = "SeesawPivot";
            pillar.transform.position = seesawPos + new Vector3(0, -1f, 0);
            pillar.transform.localScale = new Vector3(1, 1, 1);
            pillar.GetComponent<Renderer>().sharedMaterial = matPlatform;
            Rigidbody pillarRb = pillar.AddComponent<Rigidbody>();
            pillarRb.isKinematic = true;

            GameObject board = GameObject.CreatePrimitive(PrimitiveType.Cube);
            board.name = "SeesawBoard";
            board.transform.position = seesawPos;
            board.transform.localScale = new Vector3(6, 0.5f, 11);
            board.GetComponent<Renderer>().sharedMaterial = matJoints;
            Rigidbody boardRb = board.AddComponent<Rigidbody>();
            boardRb.mass = 10f; 
            
            HingeJoint hinge = board.AddComponent<HingeJoint>();
            hinge.connectedBody = pillarRb;
            hinge.axis = new Vector3(1, 0, 0);
            hinge.anchor = Vector3.zero;
            hinge.useLimits = true;
            JointLimits limits = hinge.limits;
            limits.min = -30f;
            limits.max = 30f;
            hinge.limits = limits;
        }
        currentPos += new Vector3(0, 0, 55);

        // 6. Floating Steps Climb
        CreateCheckpoint(currentPos, matCheckpoint);
        for (int i = 0; i < 10; i++)
        {
            Vector3 stepPos = currentPos + new Vector3(0, i * 1.5f, i * 3.5f);
            CreatePlatform(stepPos, new Vector3(4, 0.5f, 4), matPlatform);
        }
        currentPos += new Vector3(0, 9 * 1.5f, 9 * 3.5f + 12f);

        // 7. Triple Paddle Spinners (HingeJoint Motors)
        CreateCheckpoint(currentPos, matCheckpoint);
        CreatePlatform(currentPos, new Vector3(14, 1, 20), matPlatform);
        
        GameObject tAnchor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        tAnchor.name = "TripleAnchor";
        tAnchor.transform.position = currentPos + new Vector3(0, 0.5f, 0);
        tAnchor.transform.localScale = new Vector3(1, 0.5f, 1);
        tAnchor.GetComponent<Renderer>().enabled = false;
        Rigidbody tAnchorRb = tAnchor.AddComponent<Rigidbody>();
        tAnchorRb.isKinematic = true;

        GameObject tripleSpinner = new GameObject("TriplePaddleSpinner");
        tripleSpinner.transform.position = currentPos + new Vector3(0, 1.2f, 0); 
        Rigidbody tsRb = tripleSpinner.AddComponent<Rigidbody>();
        tsRb.mass = 80f;

        HingeJoint tHinge = tripleSpinner.AddComponent<HingeJoint>();
        tHinge.connectedBody = tAnchorRb;
        tHinge.axis = new Vector3(0, 1, 0);
        tHinge.useMotor = true;
        JointMotor tMotor = tHinge.motor;
        tMotor.targetVelocity = 120f;
        tMotor.force = 100000f; 
        tMotor.freeSpin = false;
        tHinge.motor = tMotor;

        for (int i = 0; i < 3; i++)
        {
            GameObject arm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arm.transform.SetParent(tripleSpinner.transform);
            arm.transform.localRotation = Quaternion.Euler(0, i * 120, 0);
            arm.transform.localPosition = arm.transform.localRotation * new Vector3(0, 0, 3.5f);
            arm.transform.localScale = new Vector3(0.6f, 0.5f, 7f);
            arm.GetComponent<Renderer>().sharedMaterial = matObstacle;
        }

        currentPos += new Vector3(0, 0, 20);

        // 8. Long Pendulum Walkway (HingeJoints)
        CreateCheckpoint(currentPos, matCheckpoint);
        CreatePlatform(currentPos, new Vector3(3, 1, 35), matPlatform);
        
        for (int i = 0; i < 6; i++)
        {
            Vector3 pendPos = currentPos + new Vector3(0, 8, -12 + i * 5);
            
            GameObject pendPivot = new GameObject("PendulumPivot_" + i);
            pendPivot.transform.position = pendPos;
            Rigidbody pivotRb = pendPivot.AddComponent<Rigidbody>();
            pivotRb.isKinematic = true;
            
            GameObject pendBody = new GameObject("PendulumBody_" + i);
            pendBody.transform.position = pendPos;
            Rigidbody pendRb = pendBody.AddComponent<Rigidbody>();
            pendRb.mass = 20f;
            
            HingeJoint pHinge = pendBody.AddComponent<HingeJoint>();
            pHinge.connectedBody = pivotRb;
            pHinge.axis = new Vector3(0, 0, 1);

            FG_PendulumMotor pendMotor = pendBody.AddComponent<FG_PendulumMotor>();
            pendMotor.swingSpeed = 150f + (i * 20f);
            pendMotor.limitAngle = 50f;

            GameObject pendArm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pendArm.transform.SetParent(pendBody.transform);
            pendArm.transform.localPosition = new Vector3(0, -4f, 0);
            pendArm.transform.localScale = new Vector3(0.4f, 4f, 0.4f);
            
            GameObject pendBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pendBall.transform.SetParent(pendBody.transform);
            pendBall.transform.localPosition = new Vector3(0, -8, 0);
            pendBall.transform.localScale = new Vector3(3f, 3f, 3f);
            pendBall.GetComponent<Renderer>().sharedMaterial = matObstacle;
        }

        currentPos += new Vector3(0, 0, 35);

        // 9. Drop Door Maze (HingeJoints)
        CreateCheckpoint(currentPos, matCheckpoint);
        for (int i = 0; i < 6; i++)
        {
            Vector3 rowPos = currentPos + new Vector3(0, 0, i * 4.5f);
            
            int strongIndex = Random.Range(0, 3);
            
            for(int j = 0; j < 3; j++)
            {
                Vector3 doorPos = rowPos + new Vector3(-4.5f + j * 4.5f, 0, 0);
                
                GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                pillar.transform.position = doorPos + new Vector3(-2f, -0.5f, 0);
                pillar.transform.localScale = new Vector3(0.5f, 1, 4);
                pillar.GetComponent<Renderer>().sharedMaterial = matPlatform;
                Rigidbody pillarRb = pillar.AddComponent<Rigidbody>();
                pillarRb.isKinematic = true;

                GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
                door.name = "DropDoor_" + i + "_" + j;
                door.transform.position = doorPos;
                door.transform.localScale = new Vector3(4f, 0.5f, 4f);
                door.GetComponent<Renderer>().sharedMaterial = matFakeFloor;
                Rigidbody doorRb = door.AddComponent<Rigidbody>();
                doorRb.mass = 2f;

                HingeJoint hinge = door.AddComponent<HingeJoint>();
                hinge.connectedBody = pillarRb;
                hinge.anchor = new Vector3(-0.5f, 0, 0);
                hinge.axis = new Vector3(0, 0, 1);
                
                hinge.useLimits = true;
                JointLimits limits = hinge.limits;
                limits.min = -90f; 
                limits.max = 0f;
                hinge.limits = limits;

                hinge.useSpring = true;
                JointSpring spring = hinge.spring;
                if (j == strongIndex)
                {
                    spring.spring = 500f; 
                }
                else
                {
                    spring.spring = 5f; 
                }
                spring.targetPosition = 0f;
                hinge.spring = spring;
            }
        }
        currentPos += new Vector3(0, 0, 32);

        // 10. Huge Final Climb
        CreateCheckpoint(currentPos, matCheckpoint);
        for (int i = 0; i < 8; i++)
        {
            Vector3 stepPos = currentPos + new Vector3(0, i * 1.5f, i * 4f);
            CreatePlatform(stepPos, new Vector3(3, 0.5f, 3), matPlatform);
        }
        currentPos += new Vector3(0, 7 * 1.5f, 7 * 4f + 12f);

        // 11. Final Boss Reverse Triple Spinners + Bouncers
        CreateCheckpoint(currentPos, matCheckpoint);
        CreatePlatform(currentPos, new Vector3(16, 1, 16), matPlatform);
        
        GameObject fAnchor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        fAnchor.name = "FinalAnchor";
        fAnchor.transform.position = currentPos + new Vector3(0, 0.5f, 0);
        fAnchor.transform.localScale = new Vector3(1, 0.5f, 1);
        fAnchor.GetComponent<Renderer>().enabled = false;
        Rigidbody fAnchorRb = fAnchor.AddComponent<Rigidbody>();
        fAnchorRb.isKinematic = true;

        GameObject finalSpinner = new GameObject("FinalTriplePaddle");
        finalSpinner.transform.position = currentPos + new Vector3(0, 1.2f, 0); 
        Rigidbody fsRb = finalSpinner.AddComponent<Rigidbody>();
        fsRb.mass = 80f;

        HingeJoint fsHinge = finalSpinner.AddComponent<HingeJoint>();
        fsHinge.connectedBody = fAnchorRb;
        fsHinge.axis = new Vector3(0, 1, 0);
        fsHinge.useMotor = true;
        JointMotor fsMotor = fsHinge.motor;
        fsMotor.targetVelocity = -120f; 
        fsMotor.force = 100000f;
        fsMotor.freeSpin = false;
        fsHinge.motor = fsMotor;
        
        for (int i = 0; i < 3; i++)
        {
            GameObject arm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arm.transform.SetParent(finalSpinner.transform);
            arm.transform.localRotation = Quaternion.Euler(0, i * 120, 0);
            arm.transform.localPosition = arm.transform.localRotation * new Vector3(0, 0, 3.5f);
            arm.transform.localScale = new Vector3(0.6f, 0.6f, 7f);
            arm.GetComponent<Renderer>().sharedMaterial = matObstacle;
        }

        for (int i = 0; i < 6; i++)
        {
            GameObject bouncer = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bouncer.name = "FinalBouncer_" + i;
            Vector3 offset = Quaternion.Euler(0, i * 60 + 30, 0) * new Vector3(0, 0, 6);
            bouncer.transform.position = currentPos + offset + new Vector3(0, 0.5f, 0);
            bouncer.transform.localScale = new Vector3(2, 0.5f, 2);
            bouncer.GetComponent<Renderer>().sharedMaterial = matBouncer;
            bouncer.AddComponent<FG_Bouncer>();
        }

        currentPos += new Vector3(0, 0, 20);

        // 12. Win Zone
        CreatePlatform(currentPos, new Vector3(14, 1, 14), matWin);
        GameObject winZone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        winZone.name = "WinZone";
        winZone.transform.position = currentPos + new Vector3(0, 1, 0);
        winZone.transform.localScale = new Vector3(14, 2, 14);
        winZone.GetComponent<Renderer>().enabled = false;
        winZone.GetComponent<Collider>().isTrigger = true;
        winZone.AddComponent<FG_WinZone>();

        // Scale Death Zone
        GameObject deathZone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        deathZone.name = "DeathZone";
        deathZone.transform.position = new Vector3(0, -20, currentPos.z / 2);
        deathZone.transform.localScale = new Vector3(300, 2, currentPos.z + 100);
        deathZone.GetComponent<Renderer>().enabled = false;
        deathZone.GetComponent<Collider>().isTrigger = true;
        deathZone.AddComponent<FG_DeathZone>();
        
        Debug.Log("Massive Joint-Based Fall Guys Course Generated Successfully!");
    }

    static void CreatePlatform(Vector3 pos, Vector3 scale, Material mat)
    {
        GameObject plat = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plat.name = "Platform";
        plat.transform.position = pos;
        plat.transform.localScale = scale;
        plat.GetComponent<Renderer>().sharedMaterial = mat;
    }

    static void CreateCheckpoint(Vector3 pos, Material mat)
    {
        GameObject cpBase = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cpBase.name = "CheckpointBase";
        cpBase.transform.position = pos + new Vector3(0, 0, -4.5f);
        cpBase.transform.localScale = new Vector3(10, 1f, 2f);
        cpBase.GetComponent<Renderer>().sharedMaterial = mat;

        GameObject cpTrigger = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cpTrigger.name = "CheckpointTrigger";
        cpTrigger.transform.position = pos + new Vector3(0, 1f, -4.5f);
        cpTrigger.transform.localScale = new Vector3(10, 1f, 2f);
        cpTrigger.GetComponent<Renderer>().enabled = false; 
        cpTrigger.GetComponent<Collider>().isTrigger = true;
        
        GameObject respawn = new GameObject("RespawnPoint");
        respawn.transform.SetParent(cpBase.transform);
        respawn.transform.localPosition = new Vector3(0, 1.5f, 0); 

        FG_Checkpoint cpComp = cpTrigger.AddComponent<FG_Checkpoint>();
        cpComp.respawnPoint = respawn.transform;
    }
}
