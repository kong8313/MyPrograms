export {Types} from "./actions";
import {IState as IRS, store} from "./store";
export type IState = IRS;
export {connect} from "react-redux";
export * as Actions from "./actions"
export {store} from "./store"
import {InferableComponentEnhancerWithProps} from "react-redux";

export const state = () => store.getState() as IState;
export type TypeOfConnect<T> = T extends InferableComponentEnhancerWithProps<infer Props, infer _> ? Props : never;
