//
// Created by Sofus Skovgaard on 03/05/2022.
//

#include "TemperatureSender.h"

std::string TemperatureSender::topic = TEMPERATURE_SENDER_TOPIC;

std::string TemperatureSender::Handle(std::shared_ptr<SenderCommandBase> command) {
    std::shared_ptr<TemperatureSenderCommand> _command = std::static_pointer_cast<TemperatureSenderCommand>(command);
    return std::to_string(_command->temperature);
}
