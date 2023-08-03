namespace BaseClasses {
    public class PerkFactory {
        public Perk GetPerk(string perkName) {
            switch (perkName) {
                case "Fortificate":
                    return new Fortificate();
                default:
                    return null;
            }
        }
    }
}