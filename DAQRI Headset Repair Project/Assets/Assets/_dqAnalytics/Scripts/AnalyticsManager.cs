using Amazon;
using Amazon.CognitoIdentity;
using Amazon.MobileAnalytics.MobileAnalyticsManager;
using Amazon.Util.Internal;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour {

    private static AnalyticsManager instance;

    public static AnalyticsManager Get {
        get {
            if (instance == null) {
                instance = GameObject.FindObjectOfType<AnalyticsManager> ();
            }
            return instance;
        }
    }

    public string IdentityPoolId = "YourIdentityPoolId";

    public string appId = "YourAppId";

    public string CognitoIdentityRegion = RegionEndpoint.USEast1.SystemName;

    private RegionEndpoint _CognitoIdentityRegion
    {
        get { return RegionEndpoint.GetBySystemName(CognitoIdentityRegion); }
    }

    public string AnalyticsRegion = RegionEndpoint.USEast1.SystemName;

    private RegionEndpoint _AnalyticsRegion
    {
        get { return RegionEndpoint.GetBySystemName(AnalyticsRegion); }
    }

    private MobileAnalyticsManager analyticsManager;

    private CognitoAWSCredentials _credentials;

    // Use this for initialization
    void Start()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);

        _credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoIdentityRegion);
        analyticsManager = MobileAnalyticsManager.GetOrCreateInstance(appId, _credentials,
            _AnalyticsRegion);
    }

    void OnApplicationFocus(bool focus) {
        if (analyticsManager != null) {
            if (focus) {
                analyticsManager.ResumeSession();
            } else {
                analyticsManager.PauseSession();
            }
        }
    }

    public void RecordEvent(CustomEvent customEvent)
    {
        analyticsManager.RecordEvent(customEvent);
    }

}
