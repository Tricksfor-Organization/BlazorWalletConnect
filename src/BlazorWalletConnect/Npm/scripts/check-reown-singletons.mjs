import { createRequire } from 'node:module'
import { readFileSync } from 'node:fs'

const require = createRequire(import.meta.url)
const lock = JSON.parse(readFileSync(new URL('../package-lock.json', import.meta.url), 'utf8'))
const packagePaths = Object.keys(lock.packages ?? {})
const singletonPackages = [
    '@reown/appkit-common',
    '@reown/appkit-controllers',
    '@reown/appkit-utils',
    '@walletconnect/universal-provider'
]

const errors = []

for (const packageName of singletonPackages) {
    const expectedPath = `node_modules/${packageName}`
    const installedPaths = packagePaths.filter(path =>
        path === expectedPath || path.endsWith(`/node_modules/${packageName}`))

    if (installedPaths.length !== 1 || installedPaths[0] !== expectedPath) {
        errors.push(
            `${packageName} must be installed once at ${expectedPath}; found: ${installedPaths.join(', ') || 'none'}`)
    }
}

const uuidVersion = require('../node_modules/uuid/package.json').version
if (uuidVersion !== '11.1.1') {
    errors.push(`jayson's resolved uuid must be 11.1.1; found ${uuidVersion}`)
}

if (errors.length > 0) {
    throw new Error(`Dependency integrity check failed:\n${errors.join('\n')}`)
}

console.log('Reown singleton dependencies are deduplicated and uuid is patched.')
