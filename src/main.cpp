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

bool disconnecting = false;

void setup() {
    Serial.begin(115200);

    pinMode(4, INPUT_PULLUP);

    attachInterrupt(digitalPinToInterrupt(4), [] {
        if (!disconnecting && WiFiManager::connected()) {
            disconnecting = true;
            WiFi.disconnect();
            disconnecting = false;
        }
    }, LOW);

    // wait for a serial connection.
    while (!Serial) {}

    RTCManager::configure();
    LedHelper::configure();

    ServoManager::configure();
    DHTManager::configure();

    WiFiManager::configure({"SibirienAP", "Siberia51244" });
    MQTTManager::configure({
            "10.135.16.59",
            1883,
            "arduino01",
            "guest",
            "guest",
            {
                {ToggleBlindsReceiver::topic, ToggleBlindsReceiver::Handle}
            },
            {
                    {HumiditySender::topic, HumiditySender::Handle},
                    {TemperatureSender::topic, TemperatureSender::Handle},
            }
        }
    );

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

    if (ms - last_sent >= 2000) {
        last_sent = ms;

        GatherAndSendTemperature();
        GatherAndSendHumidity();
    }
}