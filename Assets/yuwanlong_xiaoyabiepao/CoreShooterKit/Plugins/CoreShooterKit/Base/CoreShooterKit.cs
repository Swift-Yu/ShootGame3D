namespace HedgehogTeam.CoreShooterKit{

	public enum FactionAlignment { Neutral=0, Enemy=-1, Ally=1};

	public enum EntityType { Offensive, Defensive, Structure, Neutral, A,B,C,D,E,F,G,H,I,J,K, NotTraceable };
	public enum TargetPriority { Closest,Offensive, Defensive, Structure, A,B,C,D};

	public enum ProjectileType { Velocity, Instant, Physic};
	public enum DamageAt {Contact, EndOfLife};
	public enum DamageType {Simple, Area};
	public enum GuidanceType { Position, Prediction }

	public enum WeaponType {Projectile,LaserBeam}
	public enum WeaponMode { FullyAutomatic=2, Burst=1, OneShot=0, None=-1 }
	public enum MagazineType {Unit,Time};
	public enum SubEmitter {Sphere,Cylinder }

	public enum ReSpawnType {EndWave, AllKill};
	public enum SpawnType {Fixe,Random, Progressive};
	public enum SpawnShape {InCircle,OnCircle, InSphere, OnSphere};

	public enum TurretState {InRadarSeek, InPursue, InAcquisition};


}

