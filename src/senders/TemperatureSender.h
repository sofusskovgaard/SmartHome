//
// Created by Sofus Skovgaard on 03/05/2022.
//

#ifndef SMARTHOME_TEMPERATURESENDER_H
#define SMARTHOME_TEMPERATURESENDER_H

#include <string>
#include <memory>

#include "base/SenderCommandBase.h"

#define TEMPERATURE_SENDER_TOPIC "home/temperature"

struct TemperatureSenderCommand : SenderCommandBase {
    std::string topic() override {
        return TEMPERATURE_SENDER_TOPIC;
    };

    float temperature;
};


class TemperatureSender {
public:
    static std::string topic;

    static std::string Handle(std::shared_ptr<SenderCommandBase> command);
};


#endif //SMARTHOME_TEMPERATURESENDER_H
