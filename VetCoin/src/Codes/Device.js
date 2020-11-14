var Device = /** @class */ (function () {
    function Device() {
    }
    Device.isSmartphone = function () {
        if (navigator.userAgent.match(/(iPhone|iPad|iPod|Android)/i)) {
            return true;
        }
        return false;
    };
    return Device;
}());
export { Device };
//# sourceMappingURL=Device.js.map