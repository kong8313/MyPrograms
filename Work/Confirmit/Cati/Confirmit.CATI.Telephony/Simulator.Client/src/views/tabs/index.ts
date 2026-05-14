import GeneratorsTab from "./generators"
import CampaignsTab from "./campaigns"
import AgentsTab from "./agents"
import ActivitiesTab from "./activities"
import DialersTab from "./dialers"
import InboundTab from "./inbound"
import TransferTab from "./transfers"
import SoftphoneTab from "./softphone"
import React from "react"
import { RouteComponentProps } from "react-router-dom"
import GeneratorBehaviorTab from "./add-generator-behavior"
import {config} from "./../../config";

export interface IRouteInfo{
    name: string;
    component: React.ComponentType<RouteComponentProps<any>> | React.ComponentType<any>, 
}

export const routes = {
    root: {
        path: `${config.baseUrl}/`,
        component: (props) => React.createElement(GeneratorsTab),
    },
    softphone: {
        name: "Softphone Simulator",
        path: `${config.baseUrl}/softphone`,
        component: (props) => React.createElement(SoftphoneTab),
        link: () => `${config.baseUrl}/softphone`
    },
    activities: {
        name: "Activities",
        path: `${config.baseUrl}/activities`,
        component: (props) => React.createElement(ActivitiesTab),
        link: () => `${config.baseUrl}/activities` 
    },
    generators: {
        name: "Generator Behaviors",
        path: `${config.baseUrl}/generators`,
        component: (props) => React.createElement(GeneratorsTab),
        link: () => `${config.baseUrl}/generators` 
    },
    agents: {
        name: "Agents",
        path: `${config.baseUrl}/agents`,
        component: (props) => React.createElement(AgentsTab),
        link: () => `${config.baseUrl}/agents` 
    },
    campaigns: {
        name: "Campaigns",
        path: `${config.baseUrl}/campaigns`,
        component: (props) => React.createElement(CampaignsTab),
        link: () => `${config.baseUrl}/campaigns` 
    },
    dialers: {
        name: "Dialers",
        path: `${config.baseUrl}/dialers`,
        component: (props) => React.createElement(DialersTab),
        link: () => `${config.baseUrl}/dialers` 
    },
    inbound: {
        name: "Inbound",
        path: `${config.baseUrl}/inbound`,
        component: (props) => React.createElement(InboundTab),
        link: () => `${config.baseUrl}/inbound` 
    },
    transfers: {
        name: "Transfers",
        path: `${config.baseUrl}/transfers`,
        component: (props) => React.createElement(TransferTab),
        link: () => `${config.baseUrl}/transfers` 
    },
    addGeneratorBehavior: {
        path: `${config.baseUrl}/generators/:generatorName/behaviors/add`,
        component: (props) => React.createElement(GeneratorBehaviorTab, { generatorName: props.match.params.generatorName}),
        link: (generatorName: string) => `${config.baseUrl}/generators/${generatorName}/behaviors/add` 
    }
}
