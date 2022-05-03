//
// Created by Sofus Skovgaard on 02/05/2022.
//

#include "WiFiManager.h"

WiFiClient WiFiManager::__client = WiFiClient();

std::shared_ptr<WiFiManagerOptions> WiFiManager::__options;

unsigned long WiFiManager::__last_verification = 0;

void WiFiManager::configure(const WiFiManagerOptions& options) {
    __options = std::make_shared<WiFiManagerOptions>(options);
}

void WiFiManager::connect() {
    int status = WiFi.status();

    if (status == WL_NO_MODULE) {
        Serial.println(PrintHelper::ts() + "[WiFi] Communication with WiFi module failed!");
        while (true);
    }

    Serial.print(PrintHelper::ts() + "[WiFi] Attempting to connect to WPA SSID: ");
    Serial.println(__options->ssid.c_str());

    LedHelper::toggle_led();

    unsigned long lastStatusBlink = millis();

    while (status != WL_CONNECTED) {

        status = WiFi.begin(__options->ssid.c_str(), __options->password.c_str());

        unsigned long ms = millis();

        if (ms-lastStatusBlink >= 500) {
            lastStatusBlink=ms;
            LedHelper::toggle_led();
        }
    }

    __last_verification = millis();
    LedHelper::strobe_led(2000);

    Serial.print(PrintHelper::ts() + "[WiFi] Connected to the network with SSID: ");
    Serial.println(__options->ssid.c_str());
}

bool WiFiManager::connected() {
    return __client.connected();
}

void WiFiManager::verify_connection() {
    unsigned long ms = millis();
    if (ms-__last_verification>=30000) {
        __last_verification = ms;

        if (!connected()) {
            connect();
            MQTTManager::connect();
        } else {
            LedHelper::strobe_led(250);
        }
    }
}
