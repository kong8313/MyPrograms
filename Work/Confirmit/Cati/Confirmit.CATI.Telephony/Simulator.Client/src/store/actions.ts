import { ActivityInfo } from "../common/api";

export enum Types {
    CONFIGURE = "CONFIGURE",
    SET_ACTIVITIES = "SET_ACTIVITIES"
};

function create<T extends Types, D>(type: T, data: D = {} as any) {
    return {type, ...data} as const;
}

export const configure = (url: string) => create(Types.CONFIGURE, {url});
export const setActivities = (activities: ActivityInfo[]) => create(Types.SET_ACTIVITIES, {activities});

export type Action = ReturnType<typeof configure>
        | ReturnType<typeof setActivities>;

