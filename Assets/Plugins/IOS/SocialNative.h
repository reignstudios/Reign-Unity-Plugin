// -------------------------------------------------------
//  Created by Andrew Witte.
//  Copyright (c) 2013 Reign-Studios. All rights reserved.
// -------------------------------------------------------

#import <Foundation/Foundation.h>
#import "GADBannerView.h"

@interface SocialNative : NSObject
{
@public
    CGRect PopoverRect;
}

- (void)ShareImage:(Byte*)data dataLength:(int)dataLength isPNG:(bool)isPNG;
@end