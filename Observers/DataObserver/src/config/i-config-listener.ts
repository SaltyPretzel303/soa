import type { ServiceConfig } from './service-configuration'

export default interface IConfigListener {
	reload(newConfig: ServiceConfig): Promise<boolean>;
}