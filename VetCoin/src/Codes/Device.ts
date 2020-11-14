
export class Device {
    public static isSmartphone():boolean {
        if (navigator.userAgent.match(/(iPhone|iPad|iPod|Android)/i)) {
            return true;
        }

        return false;
    }

}
