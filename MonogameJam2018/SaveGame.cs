using System.IO;

namespace TimeIsUp {
	class SaveGame {
		public static void SetPlayerName() {
			var save = File.ReadAllText("save.sav").Split(',');
			save[0] = Constant.PLAYER_NAME;
			File.WriteAllText("save.sav", string.Join(",", save));
		}
		public static string GetPlayerName() {
			var save = File.ReadAllText("save.sav").Split(',');
			return save[0];
		}
		public static bool IsLevelUnlocked(int level) {
			var save = File.ReadAllText("save.sav").Split(',');
			return save[level + 1] == "1";
		}
		public static void UnlockLevel(int level) {
			var save = File.ReadAllText("save.sav").Split(',');
			save[level + 1] = "1";
			File.WriteAllText("save.sav", string.Join(",", save));
		}
	}
}
