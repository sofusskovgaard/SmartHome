//
// Created by Sofus Skovgaard on 02/05/2022.
//

#include "WiFiManager.h"

WiFiClient WiFiManager::__client = WiFiClient();

//std::shared_ptr<WiFiManagerOptions> WiFiManager::__options;

std::shared_ptr<std::map<std::string, std::string>> WiFiManager::__credentials;

unsigned long WiFiManager::__last_verification = 0;

std::shared_ptr<std::pair<std::string, std::string>> WiFiManager::__find_best_network() {

    int numberOfNetworks = WiFi.scanNetworks();
    std::shared_ptr<std::pair<std::string, std::string>> bestKnownNetwork;
    int bestKnownNetworkNumber = -1;
    int bestKnownNetworkRSSI = -1;

    for(int i=0; i < numberOfNetworks; i++) {
        auto _network = __credentials->find(WiFi.SSID(i));

        std::string ssid(WiFi.SSID(i));

        Serial.println(PrintHelper::ts() + "[WiFi] Found network with SSID -> " + ssid.c_str());

        if (__credentials->find(ssid) != __credentials->end()) {
            int rssi = WiFi.RSSI(i);

            if (bestKnownNetworkNumber == -1) {
                bestKnownNetwork = std::make_shared<std::pair<std::string, std::string>>(*_network);
                bestKnownNetworkNumber = i;
                bestKnownNetworkRSSI = rssi;
            } else if (bestKnownNetworkRSSI < rssi) {
                bestKnownNetwork = std::make_shared<std::pair<std::string, std::string>>(*_network);
                bestKnownNetworkNumber = i;
                bestKnownNetworkRSSI = rssi;
            }
        }
    }

    if (bestKnownNetwork) {
        Serial.println(PrintHelper::ts() + "[WiFi] Chose network with SSID -> " + bestKnownNetwork->first.c_str());
    } else {
        Serial.println(PrintHelper::ts() + "[WiFi] Could not find any known networks");
    }

    return bestKnownNetwork;
}

//void WiFiManager::configure(const WiFiManagerOptions& options) {
//    __options = std::make_shared<WiFiManagerOptions>(options);
//}

void WiFiManager::configure(const std::map<std::string, std::string> &credentials) {
    __credentials = std::make_shared<std::map<std::string, std::string>>(credentials);
}

void WiFiManager::connect() {
    int status = WiFi.status();

    if (status == WL_NO_MODULE) {
        Serial.println(PrintHelper::ts() + "[WiFi] Communication with WiFi module failed!");
        while (true);
    }

    std::shared_ptr<std::pair<std::string, std::string>> bestKnownNetwork;

    while (!bestKnownNetwork) {
        bestKnownNetwork = __find_best_network();
        delay(5000);
    }

    Serial.print(PrintHelper::ts() + "[WiFi] Attempting to connect to WPA SSID: ");
    Serial.println(bestKnownNetwork->first.c_str());

    LedHelper::toggle_led();

    unsigned long lastStatusBlink = millis();

    while (status != WL_CONNECTED) {

        status = WiFi.begin(bestKnownNetwork->first.c_str(), bestKnownNetwork->second.c_str());

        unsigned long ms = millis();

        if (ms-lastStatusBlink >= 500) {
            lastStatusBlink=ms;
            LedHelper::toggle_led();
        }
    }

    __last_verification = millis();
    LedHelper::strobe_led(2000);

    WiFi.lowPowerMode();

    Serial.print(PrintHelper::ts() + "[WiFi] Connected to the network with SSID: ");
    Serial.println(bestKnownNetwork->first.c_str());
}

bool WiFiManager::connected() {
    return __client.connected();
}

void WiFiManager::verify_connection() {
    unsigned long ms = millis();
    if (ms-__last_verification>=5000) {
        __last_verification = ms;

        if (!connected()) {
            connect();
            MQTTManager::connect();
        } else {
            LedHelper::strobe_led(250);
        }
    }
}
