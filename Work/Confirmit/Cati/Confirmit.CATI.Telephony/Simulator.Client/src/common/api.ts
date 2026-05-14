import {state} from "../store"

export interface GeneratorBehaviorInfo{
    id: string,
    type: "Manual"|"Value",
    value: string,
    owner: string,
    filter?: {
        companyId?: number,
        dialerId?: number,
        campaignId?: number,
        agentId?: number,
        interviewId?: number,
    }
}

export interface GeneratorInfo{
    name: string;
    behaviors: GeneratorBehaviorInfo[];
}

export interface AgentInfo{
    companyId: number;
    dialerId: number;
    agentId: number;
    name: string;
    campaignId: number;
    isPredictive: boolean;
    type: string;
}

export interface CampaignInfo{
    companyId: number;
    dialerId: number;
    campaignId: number;
    name: string;
    dialingMode: string;
}

export interface ActivityInfo{
    id: string;
    name: string;
    owner: string;
    context: { 
        campaignId: number;
        agentId: number;
        interviewId?: number;
    }
    commands: string[];
}

export interface DialerInfo{
    companyId: number;
    dialerId: number;
    initializationTime: string;
    agentsCount: number;
    campaignsCount: number;
}

export interface InboundDdiInfo{
    companyId: number;
    dialerId: number;
    number: string;
}

export interface InboundCallInfo{
    companyId: number;
    dialerId: number;
    ddiNumber: string;
    cliNumber: string;
    inboundCallId?: string;
}

export interface TransferInfo{
    companyId: number;
    dialerId: number;
    transferId: string;
    initiator: string;
    target: string;
    type : "externalCold" | "externalWarm" | "InternalCold" | "InternalWarm";
    state : {
        initiatorAgentId: number;
        initiatorState: "notDefined" | "connected" | "notConnected";
        connectionState: string;
        targetType: string;
        targetResource: string;
        targetState: string;
        targetOutcome: string;
    }
}

export async function get<T>(resource: string) : Promise<T> {
    const url = state().url + resource;
    const response = await fetch(url);
    return (await response.json()) as T;
}

export async function post<T>(resource: string, body: T) {
    const url = state().url + resource;
    const response = await fetch(url, {method: 'POST', body: JSON.stringify(body), headers: { 'Content-Type': 'application/json'}});
    if(response.status > 400)
        throw `post: ${response.statusText}(${response.status})`
}

export async function del(resource: string) {
    const url = state().url + resource;
    const response = await fetch(url, {method: 'DELETE'});
}


export const getGenerators = () => get<GeneratorInfo[]>('generators');

export const getAgents = () => get<AgentInfo[]>('agents');

export const getCampaigns = () => get<CampaignInfo[]>('campaigns');

export const getActivities = () => get<ActivityInfo[]>('activities');

export const getDialers = () => get<DialerInfo[]>('dialers');

export const getInboundDDI = () => get<InboundDdiInfo[]>('inbound/ddi');

export const getInboundCalls = () => get<InboundCallInfo[]>('inbound/calls');

export const getTransfers = () => get<TransferInfo[]>('transfers');


export const addGeneratorBehavior = (generatorName: string, behavior: GeneratorBehaviorInfo) => post<GeneratorBehaviorInfo>(`generators/${generatorName}/behaviors`, behavior)

export const createInboundCall = (inbound: InboundCallInfo) => post<InboundCallInfo>(`inbound/calls`, inbound)

export const dropInboundCall = (companyId: number, dialerId: number, inboundCallId: string) => del(`inbound/calls?companyId=${companyId}&dialerId=${dialerId}&inboundCallId=${inboundCallId}`)

export const deleteAgent = (companyId: number, dialerId: number, agentId: number) => del(`agents?companyId=${companyId}&dialerId=${dialerId}&agentId=${agentId}`);

export const deleteGeneratorBehavior = (generatorName: string, id: string) => del(`generators/${generatorName}/behaviors/${id}`);

export const executeCommand = (id: string, command: string, args: string) => post(`activities/${id}?command=${command}&args=${args}`, null);