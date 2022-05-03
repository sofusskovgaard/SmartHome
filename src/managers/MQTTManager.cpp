//
// Created by Sofus Skovgaard on 02/05/2022.
//

#include "MQTTManager.h"

std::shared_ptr<MQTTManagerOptions> MQTTManager::__options;
MQTTClient MQTTManager::__mqtt = MQTTClient();

std::shared_ptr<std::map<std::string, std::function<void(std::string)>>> MQTTManager::__receivers;

std::shared_ptr<std::map<std::string, std::function<std::string(std::shared_ptr<SenderCommandBase>)>>> MQTTManager::__senders;

std::queue<std::shared_ptr<SenderCommandBase>> MQTTManager::__commands;

void MQTTManager::configure(const MQTTManagerOptions& options) {
    __options = std::make_shared<MQTTManagerOptions>(options);

    __receivers = std::make_shared<std::map<std::string, std::function<void(std::string)>>>(__options->receivers);
    __senders = std::make_shared<std::map<std::string, std::function<std::string(std::shared_ptr<SenderCommandBase>)>>>(__options->senders);
}

void MQTTManager::message_received(String &topic, String &payload) {
    LedHelper::led_on();

    auto result = __receivers->find(topic.c_str());

    if (result != __receivers->end()) {
        result->second(payload.c_str());
    }

    LedHelper::led_off();
}

void MQTTManager::connect() {
    __mqtt.begin(__options->hostname.c_str(), 1883, WiFiManager::client());
    __mqtt.onMessage(MQTTManager::message_received);

    Serial.println(PrintHelper::ts() + "[MQTT] Connecting...");

    while (!__mqtt.connect(
            __options->client_id.c_str(),
            __options->username.c_str(),
            __options->password.c_str()
            )) {}

    Serial.println(PrintHelper::ts() + "[MQTT] Connected!");

    __subscribe_receivers();
}

void MQTTManager::loop() {
    __mqtt.loop();
    __handle_senders();
}

void MQTTManager::__subscribe_receivers() {
    std::map<std::string, std::function<void(std::string)>>::iterator item;

    for (item = __receivers->begin(); item != __receivers->end(); item++) {
        __subscribe_receiver(item->first);
    }
}

void MQTTManager::__subscribe_receiver(std::string topic) {
    Serial.println(PrintHelper::ts() + "[MQTT] Subscribing to topic -> " + topic.c_str());
    __mqtt.subscribe(topic.c_str());
}

void MQTTManager::__handle_senders() {
    if (!__commands.empty() && WiFiManager::connected()) {
        std::shared_ptr<SenderCommandBase> command = __commands.front();
        Serial.println(PrintHelper::ts() + "[MQTT] Found command for topic -> " + command->topic().c_str() + " collected " + (RTCManager::get_rtc().now()-command->collected_at).totalseconds() + " seconds ago");
        auto result = __senders->find(command->topic());
        if (result != __senders->end()) {
            std::string response = result->second(command);
            bool mqtt_result = __mqtt.publish(command->topic().c_str(), response.c_str());

            if (mqtt_result) {
                Serial.println(PrintHelper::ts() + "[MQTT] Successfully handled command for topic -> " + command->topic().c_str());
            } else {
                    Serial.println(PrintHelper::ts() + "[MQTT] Failed to handle command for topic -> " + command->topic().c_str());
            }
        } else {
            Serial.println(PrintHelper::ts() + "[MQTT] Couldn't find handler for topic -> " + command->topic().c_str());
        }
        __commands.pop();
    }
}

void MQTTManager::send_command(const std::shared_ptr<SenderCommandBase>& command) {
    __commands.push(command);
}
