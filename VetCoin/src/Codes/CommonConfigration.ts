

export default class CommonConfiguration {

    static getBaseUrl(): string {
        const urlSplit = location.href.split('/');
        let apiBaseUrl = urlSplit[0] + "//" + urlSplit[2];
        //ローカルホストの場合開発サーバーを向ける

        if (location.href.startsWith("http://localhost:80")) {
            apiBaseUrl = "https://localhost:44392";
        }

        return apiBaseUrl;
    }
} 
 