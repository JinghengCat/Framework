namespace Core.Res
{
    public class EAssetType
    {
        public const int AssetBundle = 0;
        public const int Prefab = 1;
        public const int Material = 2;
        public const int Asset = 3;
    }

    public class ELoadAssetErrorCode
    {
        public const int SUCCESS = 0;
        public const int FILE_NOT_EXIST = -1;
        public const int REPEAT_LOAD = -2;
        public const int LOAD_FAILED = -3;
    }

    public class EAssetSourceType
    {
        public const int PROJECT = 0;
        public const int DOWNLOAD = 1;
    }
    
    public enum ELoadState
    {
        None,
        Loading,
        Loaded,
    }
}
