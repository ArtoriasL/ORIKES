/// Writen by Boris Chuprin smokerr@mail.ru
/// Great gratitude to everyone who helps me to convert it to C#
/// Thank you so much !!
using UnityEngine;
using System;



public class BikerAnim : MonoBehaviour
{
    /// Writen by Boris Chuprin smokerr@mail.ru
    Animator myAnimator;

    // variables for turn IK link off for a time
    private int IK_rightWeight = 1;
    private int IK_leftWeight = 1;

    //variables for moving right/left and forward/backward
    private float bikerLeanAngle = 0.0f;
    private float bikerMoveAlong = 0.0f;

    //variables for moving reverse animation
    private float reverseSpeed = 0.0f;

    // point of head tracking for(you may disable it or put it on any object you want - the rider will look on that object)
    private Transform lookPoint;

    // standard point of interest for a head
    public Transform camPoint;

    // this point may be throwed to anything you want rider looking at
    public Transform poi01;//for example now it's a gameObject "car". It means when rider near a "car"(distanceToPoi = 50 meters) he will look at "car" until he move far than 50 meters.

    //the distance rider will look for POI(point of interest)
    public float distanceToPoi; //in meters = 50 by default

    // variables for hand IK joint points
    public Transform IK_rightHandTarget;
    public Transform IK_leftHandTarget;

    //ragdoll define
    public GameObject ragDoll;

    //variable for only one ragdoll create when crashed
    bool ragdollLaunched = false;

    //fake joint for physical movement biker to imitate inertia
    public Transform fakeCharPhysJoint;

    //we need to know bike we ride on
    public GameObject bikeRideOn;
    //why it's here ? because you might use this script with many bikes in one scene

    private BikeController bikeController;// making a link to corresponding bike's script
    //why it's here ? because you might use this script with many bikes in one scene

    GameObject ctrlHub;// gameobject with script control variables 
    controlHub outsideControls;// making a link to corresponding bike's script

    public void Start()
    {

        ctrlHub = GameObject.Find("gameScenario");//link to GameObject with script "controlHub"
        outsideControls = ctrlHub.GetComponent<controlHub>();//to connect c# mobile control script to this one

        myAnimator = GetComponent<Animator>();
        lookPoint = camPoint;//use it if you want to rider look at anything when riding

        //need to know when bike crashed to launch a ragdoll
        bikeController = bikeRideOn.GetComponent<BikeController>();
        myAnimator.SetLayerWeight(2, 0.0f); //to turn off layer with reverse animation which override all other

    }

    //fundamental mecanim IK script
    //just keeps hands on wheelbar :)
    public void OnAnimatorIK(int layerIndex)
    {
        if (IK_rightHandTarget != null)
        {
            myAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, (float)IK_rightWeight);
            myAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, (float)IK_rightWeight);
            myAnimator.SetIKPosition(AvatarIKGoal.RightHand, IK_rightHandTarget.position);
            myAnimator.SetIKRotation(AvatarIKGoal.RightHand, IK_rightHandTarget.rotation);
        }
        if (IK_leftHandTarget != null)
        {
            myAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, (float)IK_leftWeight);
            myAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, (float)IK_leftWeight);
            myAnimator.SetIKPosition(AvatarIKGoal.LeftHand, IK_leftHandTarget.position);
            myAnimator.SetIKRotation(AvatarIKGoal.LeftHand, IK_leftHandTarget.rotation);
        }
        //same for a head
        myAnimator.SetLookAtPosition(lookPoint.transform.position);
        myAnimator.SetLookAtWeight(0.5f);//0.5f - means it rotates head 50% mixed with real animations 
    }



    public void Update()
    {
        //moves character with fake inertia
        if (fakeCharPhysJoint != null)
        {
            var tmp_cs1 = this.transform.localEulerAngles;
            tmp_cs1.x = fakeCharPhysJoint.localEulerAngles.x;
            tmp_cs1.y = fakeCharPhysJoint.localEulerAngles.y;
            tmp_cs1.z = fakeCharPhysJoint.localEulerAngles.z;
            this.transform.localEulerAngles = tmp_cs1;
        }
        else return;

        //the character should play animations when player press control keys
        //horizontal movement
        if (outsideControls.Horizontal < 0 && bikerLeanAngle > -1.0f)
        {
            bikerLeanAngle -= 8 * Time.deltaTime;//8 - "magic number" of speed of pilot's body movement across. Just 8 - face it :)
            if (bikerLeanAngle < outsideControls.Horizontal) bikerLeanAngle = outsideControls.Horizontal;//this string seems strange but it's necessary for mobile version
            myAnimator.SetFloat("lean", bikerLeanAngle);//the character play animation "lean" for bikerLeanAngle more and more
        }
        if (outsideControls.Horizontal > 0 && bikerLeanAngle < 1.0f)
        {
            bikerLeanAngle += 8 * Time.deltaTime;
            if (bikerLeanAngle > outsideControls.Horizontal) bikerLeanAngle = outsideControls.Horizontal;
            myAnimator.SetFloat("lean", bikerLeanAngle);
        }
        //vertical movement
        if (outsideControls.Vertical > 0 && bikerMoveAlong < 1.0f)
        {
            //bikerMoveAlong += 3 * Time.deltaTime;
            if (bikerMoveAlong > outsideControls.Vertical) bikerMoveAlong = outsideControls.Vertical;
            myAnimator.SetFloat("moveAlong", bikerMoveAlong);
        }
        if (outsideControls.Vertical < 0 && bikerMoveAlong > -1.0f)
        {
            bikerMoveAlong -= 3 * Time.deltaTime;
            if (bikerMoveAlong < outsideControls.Vertical) bikerMoveAlong = outsideControls.Vertical;
            myAnimator.SetFloat("moveAlong", bikerMoveAlong);
        }
        //pilot's mass tranform movement
        if (outsideControls.HorizontalMassShift < 0 && bikerLeanAngle > -1.0f)
        {
            bikerLeanAngle -= 6 * Time.deltaTime;
            if (bikerLeanAngle < outsideControls.HorizontalMassShift) bikerLeanAngle = outsideControls.HorizontalMassShift;
            myAnimator.SetFloat("lean", bikerLeanAngle);
        }
        if (outsideControls.HorizontalMassShift > 0 && bikerLeanAngle < 1.0f)
        {
            bikerLeanAngle += 6 * Time.deltaTime;
            if (bikerLeanAngle > outsideControls.HorizontalMassShift) bikerLeanAngle = outsideControls.HorizontalMassShift;
            myAnimator.SetFloat("lean", bikerLeanAngle);
        }
        if (outsideControls.VerticalMassShift > 0 && bikerMoveAlong < 1.0f)
        {
            bikerMoveAlong += 3 * Time.deltaTime;
            if (bikerLeanAngle > outsideControls.VerticalMassShift) bikerLeanAngle = outsideControls.VerticalMassShift;
            myAnimator.SetFloat("moveAlong", bikerMoveAlong);
        }
        if (outsideControls.VerticalMassShift < 0 && bikerMoveAlong > -1.0f)
        {
            bikerMoveAlong -= 3 * Time.deltaTime;
            if (bikerLeanAngle < outsideControls.VerticalMassShift) bikerLeanAngle = outsideControls.VerticalMassShift;
            myAnimator.SetFloat("moveAlong", bikerMoveAlong);
        }

        //in a case of restart
        if (outsideControls.restartBike)
        {
            //delete ragdoll when restarting scene
            GameObject RGtoDestroy = GameObject.Find("char_ragDoll(Clone)");
            Destroy(RGtoDestroy);
            //make character visible again
            Transform riderBodyVis = transform.Find("root/Hips");
            riderBodyVis.gameObject.SetActive(true);
            //now we can crash again
            ragdollLaunched = false;
        }

        //function for avarage rider pose
        bikerComeback();

        //in case of crashed call ragdoll
        if (bikeRideOn.transform.name == "rigid_bike")
        {
            if (bikeController.crashed && !ragdollLaunched)
            {
                createRagDoll();
            }
        }

        //scan do rider see POI
        if (poi01.gameObject.activeSelf && distanceToPoi > Vector3.Distance(this.transform.position, poi01.transform.position))
        {
            lookPoint = poi01;
            //if not - still looking forward for a rigidbody POI right before bike
        }
        else lookPoint = camPoint;

        // pull leg(s) down when bike stopped
        float legOffValue = 0.0f;
        if (Mathf.Round((bikeRideOn.GetComponent<Rigidbody>().velocity.magnitude * 3.6f) * 10) * 0.1f <= 15 && !bikeController.isReverseOn)
        {//no reverse speed
            reverseSpeed = 0.0f;
            myAnimator.SetFloat("reverseSpeed", reverseSpeed);

            if (bikeRideOn.transform.localEulerAngles.z <= 10 || bikeRideOn.transform.localEulerAngles.z >= 350)
            {
                if (bikeRideOn.transform.localEulerAngles.x <= 10 || bikeRideOn.transform.localEulerAngles.x >= 350)
                {
                    legOffValue = (15 - (Mathf.Round((bikeRideOn.GetComponent<Rigidbody>().velocity.magnitude * 3.6f) * 10) * 0.1f)) / 15;//need to define right speed to begin put down leg(s)
                    myAnimator.SetLayerWeight(3, legOffValue);//leg is no layer 3 in animator
                }
            }
        }
        //when using reverse speed
        if (Mathf.Round((bikeRideOn.GetComponent<Rigidbody>().velocity.magnitude * 3.6f) * 10) * 0.1f <= 15 && bikeController.isReverseOn)
        {//reverse speed

            myAnimator.SetLayerWeight(3, legOffValue);
            myAnimator.SetLayerWeight(2, 1.0f); //to turn on layer with reverse animation which override all other

            reverseSpeed = bikeController.bikeSpeed / 3;
            myAnimator.SetFloat("reverseSpeed", reverseSpeed);
            if (reverseSpeed >= 1.0f)
            {
                reverseSpeed = 1.0f;
            }

            myAnimator.speed = reverseSpeed;
        }
        else
        if (Mathf.Round((bikeRideOn.GetComponent<Rigidbody>().velocity.magnitude * 3.6f) * 10) * 0.1f > 15)
        {
            reverseSpeed = 0.0f;
            myAnimator.SetFloat("reverseSpeed", reverseSpeed);
            myAnimator.SetLayerWeight(3, legOffValue);
            myAnimator.SetLayerWeight(2, 0.0f); //to turn off layer with reverse animation which override all other
            myAnimator.speed = 1.0f;
        }


    }

    public void bikerComeback()
    {
        if (outsideControls.Horizontal == 0 && outsideControls.HorizontalMassShift == 0)
        {
            if (bikerLeanAngle > 0)
            {
                bikerLeanAngle -= 6 * Time.deltaTime;//6 - "magic number" of speed of pilot's body movement back across. Just 6 - face it :)
                myAnimator.SetFloat("lean", bikerLeanAngle);
            }
            if (bikerLeanAngle < 0)
            {
                bikerLeanAngle += 6 * Time.deltaTime;
                myAnimator.SetFloat("lean", bikerLeanAngle);
            }
        }
        if (outsideControls.Vertical == 0 && outsideControls.VerticalMassShift == 0)
        {
            if (bikerMoveAlong > 0)
            {
                bikerMoveAlong -= 2 * Time.deltaTime;//3 - "magic number" of speed of pilot's body movement back along. Just 3 - face it :)
                myAnimator.SetFloat("moveAlong", bikerMoveAlong);
            }
            if (bikerMoveAlong < 0)
            {
                bikerMoveAlong += 2 * Time.deltaTime;
                myAnimator.SetFloat("moveAlong", bikerMoveAlong);
            }
        }
    }

    //creating regdoll(we need to scan every bone of character when crashed and copy that preset to created ragdoll)
    public void createRagDoll()
    {
        if (!ragdollLaunched)
        {
            Transform pilotHips = transform.Find("root/Hips");
            Transform pilotChest = transform.Find("root/Hips/Spine/Chest");
            Transform pilotHead = transform.Find("root/Hips/Spine/Chest/Neck/Head");
            Transform pilotLeftArm = transform.Find("root/Hips/Spine/Chest/lShoulder/lArm");
            Transform pilotLeftForeArm = transform.Find("root/Hips/Spine/Chest/lShoulder/lArm/lForearm");
            Transform pilotRightArm = transform.Find("root/Hips/Spine/Chest/rShoulder/rArm");
            Transform pilotRightForeArm = transform.Find("root/Hips/Spine/Chest/rShoulder/rArm/rForearm");
            Transform pilotLeftUpperLeg = transform.Find("root/Hips/lUpperLeg");
            Transform pilotLeftLeg = transform.Find("root/Hips/lUpperLeg/lLeg");
            Transform pilotRightUpperLeg = transform.Find("root/Hips/rUpperLeg");
            Transform pilotRightLeg = transform.Find("root/Hips/rUpperLeg/rLeg");
            // looking for an current angles of bones rotation
            Quaternion pilotHipsAngle = pilotHips.transform.localRotation;
            Quaternion pilotChestAngle = pilotChest.transform.localRotation;
            Quaternion pilotHeadAngle = pilotHead.transform.localRotation;
            Quaternion pilotLeftArmAngle = pilotLeftArm.transform.localRotation;
            Quaternion pilotLeftForeArmAngle = pilotLeftForeArm.transform.localRotation;
            Quaternion pilotRightArmAngle = pilotRightArm.transform.localRotation;
            Quaternion pilotRightForeArmAngle = pilotRightForeArm.transform.localRotation;
            Quaternion pilotLeftUpperLegAngle = pilotLeftUpperLeg.transform.localRotation;
            Quaternion pilotLeftLegAngle = pilotLeftLeg.transform.localRotation;
            Quaternion pilotRightUpperLegAngle = pilotRightUpperLeg.transform.localRotation;
            Quaternion pilotRightLegAngle = pilotRightLeg.transform.localRotation;
            //hiding the rider
            Transform riderBodyVis = transform.Find("root/Hips");
            Vector3 currentPilotPosition = this.transform.position;
            Quaternion currentPilotRotation = this.transform.rotation;
            riderBodyVis.gameObject.SetActive(false);
            // creating ragdoll
            Instantiate(ragDoll, currentPilotPosition, currentPilotRotation);
            // new empty varables to fill it with learned angles later
            Transform RDpilotHips = ragDoll.transform.Find("root/Hips");
            Transform RDpilotChest = ragDoll.transform.Find("root/Hips/Spine/Chest");
            Transform RDpilotHead = ragDoll.transform.Find("root/Hips/Spine/Chest/Neck/Head");
            Transform RDpilotLeftArm = ragDoll.transform.Find("root/Hips/Spine/Chest/lShoulder/lArm");
            Transform RDpilotLeftForeArm = ragDoll.transform.Find("root/Hips/Spine/Chest/lShoulder/lArm/lForearm");
            Transform RDpilotRightArm = ragDoll.transform.Find("root/Hips/Spine/Chest/rShoulder/rArm");
            Transform RDpilotRightForeArm = ragDoll.transform.Find("root/Hips/Spine/Chest/rShoulder/rArm/rForearm");
            Transform RDpilotLeftUpperLeg = ragDoll.transform.Find("root/Hips/lUpperLeg");
            Transform RDpilotLeftLeg = ragDoll.transform.Find("root/Hips/lUpperLeg/lLeg");
            Transform RDpilotRightUpperLeg = ragDoll.transform.Find("root/Hips/rUpperLeg");
            Transform RDpilotRightLeg = ragDoll.transform.Find("root/Hips/rUpperLeg/rLeg");
            // copy known angles to new bones
            RDpilotHips.localRotation = pilotHipsAngle;
            RDpilotChest.localRotation = pilotChestAngle;
            RDpilotHead.localRotation = pilotHeadAngle;
            RDpilotLeftArm.localRotation = pilotLeftArmAngle;
            RDpilotLeftForeArm.localRotation = pilotLeftForeArmAngle;
            RDpilotRightArm.localRotation = pilotRightArmAngle;
            RDpilotRightForeArm.localRotation = pilotRightForeArmAngle;
            RDpilotLeftUpperLeg.localRotation = pilotLeftUpperLegAngle;
            RDpilotLeftLeg.localRotation = pilotLeftLegAngle;
            RDpilotRightUpperLeg.localRotation = pilotRightUpperLegAngle;
            RDpilotRightLeg.localRotation = pilotRightLegAngle;
            ragdollLaunched = true;

            if (bikeRideOn.transform.name == "rigid_bike" && !bikeController.crashed)
            {//check for crahsed status
                bikeController.crashed = true;
                bikeController.m_body.centerOfMass = new Vector3(0, -0.2f, 0);
            }
        }
    }    
}
