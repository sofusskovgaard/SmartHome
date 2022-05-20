#include <Arduino.h>

// managers
#include "managers/WiFiManager.h"
#include "managers/MQTTManager.h"
#include "managers/ServoManager.h"
#include "managers/RTCManager.h"
#include "managers/DHTManager.h"

// receivers
#include "receivers/ToggleBlindsReceiver.h"

// senders
#include "senders/HumiditySender.h"
#include "senders/TemperatureSender.h"

#define WIFI_SSID "SibirienAP"
#define WIFI_PASSWORD "Siberia51244"

#define BROKER_HOST "10.135.16.59"
#define BROKER_PORT 1883
#define MQTT_CLIENT_ID "arduino01"
#define MQTT_USERNAME "guest"
#define MQTT_PASSWORD "guest"

#include <chrono>

void setup() {
    Serial.begin(115200);

    // wait for a serial connection.
//    while (!Serial) {}

    RTCManager::configure();
    LedHelper::configure();

    ServoManager::configure();
    DHTManager::configure();

    WiFiManager::configure({
       {
               WIFI_SSID,
               WIFI_PASSWORD
       }
    });
    MQTTManager::configure({
        BROKER_HOST,
        BROKER_PORT,
        MQTT_CLIENT_ID,
        MQTT_USERNAME,
        MQTT_PASSWORD,
        {
            {ToggleBlindsReceiver::topic, ToggleBlindsReceiver::Handle}
        },
        {
            {HumiditySender::topic, HumiditySender::Handle},
            {TemperatureSender::topic, TemperatureSender::Handle},
        }
    });

    WiFiManager::connect();
    MQTTManager::connect();
}

void GatherAndSendTemperature() {
    std::shared_ptr<TemperatureSenderCommand> command = std::make_shared<TemperatureSenderCommand>();
    command->temperature = DHTManager::read_temperature();
    MQTTManager::send_command(command);
}

void GatherAndSendHumidity() {
    std::shared_ptr<HumiditySenderCommand> command = std::make_shared<HumiditySenderCommand>();
    command->humidity = DHTManager::read_humidity();
    MQTTManager::send_command(command);
}

unsigned long last_sent = 0;

void loop() {
    WiFiManager::verify_connection();
    MQTTManager::loop();

    unsigned long ms = millis();

    if (ms - last_sent >= 5000) {
        last_sent = ms;

        GatherAndSendTemperature();
        GatherAndSendHumidity();
    }
}