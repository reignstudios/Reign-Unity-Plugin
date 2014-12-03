// -------------------------------------------------------
//  Created by Andrew Witte.
//  Copyright (c) 2013 Reign-Studios. All rights reserved.
// -------------------------------------------------------

#import "iAd_AdsNative.h"
#import "UnityTypes.h"

@implementation iAd_AdsNative
- (id)init
{
    self = [super init];
    // init other data here...
    events = [[NSMutableArray alloc] init];
    return self;
}

- (void)dealloc
{
    // dispose...
    if (iAd != nil)
    {
        iAd.delegate = nil;
        [iAd removeFromSuperview];
        [iAd release];
        iAd = nil;
    }
    
    [events dealloc];
    [super dealloc];
}

- (BOOL)bannerViewActionShouldBegin:(ADBannerView*)banner willLeaveApplication:(BOOL)willLeave
{
    [events addObject:@"Clicked"];
    return YES;
}

- (void)bannerViewActionDidFinish:(ADBannerView *)banner
{
    // Called after a banner view finishes executing an action that covered your application’s user interface.
}

- (void)bannerViewWillLoadAd:(ADBannerView*)banner
{
    // ad about to loaded
}

- (void)bannerViewDidLoadAd:(ADBannerView*)banner
{
    // loaded ad
    [events addObject:@"Refreshed"];
    banner.hidden = !visible;
}

- (void)bannerView:(ADBannerView*)banner didFailToReceiveAdWithError:(NSError*)error
{
    // failed to get ad
    [events addObject:[NSString stringWithFormat:@"Error:%@", [error localizedDescription]]];
    banner.hidden = !testing || !visible;
    NSLog(@"%@",[error localizedDescription]);
}

- (void)CreateAd:(int)gravity
{
    iAd = [[ADBannerView alloc] init];
    
    // get view size
    UIViewController* view = UnityGetGLViewController();
    CGSize viewSize = view.view.frame.size;
    UIDeviceOrientation orientation = [[UIDevice currentDevice] orientation];
    bool isLandscape = false;
    if (orientation == UIDeviceOrientationLandscapeLeft || orientation == UIDeviceOrientationLandscapeRight)
    {
        isLandscape = true;
        if (viewSize.width < viewSize.height)
        {
            float width = viewSize.width;
            viewSize.width = viewSize.height;
            viewSize.height = width;
        }
    }
    
    // set ad position
    [self SetGravity:viewSize gravity:gravity];
    
    // finish
    [iAd setAutoresizingMask:UIViewAutoresizingFlexibleWidth];
    iAd.requiredContentSizeIdentifiers = [NSSet setWithObject:(isLandscape ? ADBannerContentSizeIdentifierLandscape : ADBannerContentSizeIdentifierPortrait)];
    iAd.currentContentSizeIdentifier = isLandscape ? ADBannerContentSizeIdentifierLandscape : ADBannerContentSizeIdentifierPortrait;

    view.view.window.backgroundColor = [UIColor whiteColor];
    self.window.backgroundColor = [UIColor whiteColor];
    iAd.backgroundColor = [UIColor whiteColor];
    iAd.delegate = self;
    [view.view addSubview:iAd];
}

- (void)SetGravity:(CGSize)viewSize gravity:(int)gravity
{
    CGSize size = [iAd sizeThatFits:viewSize];
    auto frame = iAd.frame;
    switch (gravity)
    {
        case 0: frame.origin = CGPointMake(0, viewSize.height-size.height); break;
        case 1: frame.origin = CGPointMake(viewSize.width-size.width, viewSize.height-size.height); break;
        case 2: frame.origin = CGPointMake((viewSize.width*.5f)-(size.width*.5f), viewSize.height-size.height); break;
        case 3: frame.origin = CGPointMake(0, 0); break;
        case 4: frame.origin = CGPointMake(viewSize.width-size.width, 0); break;
        case 5: frame.origin = CGPointMake((viewSize.width*.5f)-(size.width*.5f), 0); break;
        case 6: frame.origin = CGPointMake((viewSize.width*.5f)-(size.width*.5f), (viewSize.height*.5f)-(size.height*.5f)); break;
    }
    
    iAd.frame = frame;
}

- (void)SetVisible:(BOOL)visibleValue
{
    self->visible = visibleValue;
    iAd.hidden = !visibleValue;
}
@end

// ----------------------------------
// Unity C Link
// ----------------------------------
extern "C"
{
    iAd_AdsNative* iAd_InitAd(bool testing)
    {
        iAd_AdsNative* ad = [[iAd_AdsNative alloc] init];
        ad->testing = testing;
        return ad;
    }
    
    void iAd_DisposeAd(iAd_AdsNative* ad)
    {
        if (ad != nil) [ad release];
    }
    
    void iAd_CreateAd(iAd_AdsNative* ad, int gravity)
    {
        [ad CreateAd:gravity];
    }
    
    void iAd_SetAdGravity(iAd_AdsNative* ad, int gravity)
    {
        UIViewController* view = UnityGetGLViewController();
        [ad SetGravity:view.view.frame.size gravity:gravity];
    }
    
    void iAd_SetAdVisible(iAd_AdsNative* ad, BOOL visible)
    {
        [ad SetVisible:visible];
    }
    
    bool iAd_AdHasEvents(iAd_AdsNative* ad)
    {
        return ad->events.count != 0;
    }
    
    const char* iAd_GetNextAdEvent(iAd_AdsNative* ad)
    {
        const char* ptr = [[ad->events objectAtIndex:0] cStringUsingEncoding:NSUTF8StringEncoding];
        [ad->events removeObjectAtIndex:0];
        return ptr;
    }
}