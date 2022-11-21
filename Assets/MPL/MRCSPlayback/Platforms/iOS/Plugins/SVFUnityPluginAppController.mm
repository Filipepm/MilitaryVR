#import "UnityAppController.h"

void SVFUnityPluginLoad(IUnityInterfaces* unityInterfaces);
void SVFUnityPluginUnload();

@interface SVFUnityPluginAppController : UnityAppController
@end

@implementation SVFUnityPluginAppController
- (void)shouldAttachRenderDelegate {
    UnityRegisterRenderingPluginV5(SVFUnityPluginLoad, SVFUnityPluginUnload);
}
@end
IMPL_APP_CONTROLLER_SUBCLASS(SVFUnityPluginAppController);
