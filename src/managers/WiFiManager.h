//
// Created by Sofus Skovgaard on 02/05/2022.
//

#ifndef SMARTHOME_WIFIMANAGER_H
#define SMARTHOME_WIFIMANAGER_H

#include "WiFi.h"
#include <string>
#include <memory>

#include "managers/MQTTManager.h"

#include "helpers/LedHelper.h"
#include "helpers/PrintHelper.h"

struct WiFiManagerOptions {
    std::string ssid;
    std::string password;
};

class WiFiManager {
private:
    static WiFiClient __client;

    static std::shared_ptr<WiFiManagerOptions> __options;
    static unsigned long __last_verification;

public:
    static void configure(const WiFiManagerOptions& options);
    static void connect();
    static bool connected();
    static void verify_connection();

    static WiFiClient &client() {
        return __client;
    }
};


#endif //SMARTHOME_WIFIMANAGER_H
