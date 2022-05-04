//
// Created by Sofus Skovgaard on 03/05/2022.
//

#ifndef SMARTHOME_HUMIDITYSENDER_H
#define SMARTHOME_HUMIDITYSENDER_H

#include <string>
#include <memory>

#include "base/SenderCommandBase.h"

#define HUMIDITY_SENDER_TOPIC "home/humidity"

struct HumiditySenderCommand : SenderCommandBase {
    std::string topic() override {
        return HUMIDITY_SENDER_TOPIC;
    };

    float humidity;
};

class HumiditySender {
public:
    static std::string topic;

    static std::string Handle(std::shared_ptr<SenderCommandBase> command);
};


#endif //SMARTHOME_HUMIDITYSENDER_H
