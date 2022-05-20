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

//    static std::shared_ptr<WiFiManagerOptions> __options;
    static std::shared_ptr<std::map<std::string, std::string>> __credentials;
    static unsigned long __last_verification;

    static std::shared_ptr<std::pair<std::string, std::string>> __find_best_network();

public:
//    static void configure(const WiFiManagerOptions& options);
    static void configure(const std::map<std::string, std::string>& credentials);
    static void connect();
    static bool connected();
    static void verify_connection();

    static WiFiClient &client() {
        return __client;
    }
};


#endif //SMARTHOME_WIFIMANAGER_H
