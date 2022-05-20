//
// Created by Sofus Skovgaard on 02/05/2022.
//

#ifndef SMARTHOME_MQTTMANAGER_H
#define SMARTHOME_MQTTMANAGER_H

//#include <Arduino.h>
#include <string>
#include <map>
#include <queue>
#include <memory>

#include "MQTTClient.h"

#include "WiFiManager.h"

#include "helpers/PrintHelper.h"

#include "base/SenderCommandBase.h"

struct MQTTManagerOptions {
    std::string hostname;
    int port;
    std::string client_id;
    std::string username;
    std::string password;
    std::map<std::string, std::function<void(std::string)>> receivers;
    std::map<std::string, std::function<std::string(std::shared_ptr<SenderCommandBase>)>> senders;
};

class MQTTManager {
    static std::shared_ptr<MQTTManagerOptions> __options;
    static MQTTClient __mqtt;

    static std::shared_ptr<std::map<std::string, std::function<void(std::string)>>> __receivers;
    static std::shared_ptr<std::map<std::string, std::function<std::string(std::shared_ptr<SenderCommandBase>)>>> __senders;
    static std::queue<std::shared_ptr<SenderCommandBase>> __commands;

    static void __subscribe_receivers();
    static void __subscribe_receiver(std::string topic);

    static void __handle_senders();

public:
    static void configure(const MQTTManagerOptions& options);

    static void message_received(String &topic, String &payload);

    static void connect();

    static MQTTClient &client() {
        return __mqtt;
    };

    static void loop();

    static void send_command(const std::shared_ptr<SenderCommandBase>& command);
};


#endif //SMARTHOME_MQTTMANAGER_H
